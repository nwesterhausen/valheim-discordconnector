<#
.SYNOPSIS
    Runs DiscordConnector's in-server tests against a local Valheim dedicated server.

.DESCRIPTION
    1. Builds the mod and the test plugin.
    2. Copies both into the server's BepInEx\plugins folder.
    3. Writes test config (webhook pointed at the local capture tool, test leaderboards).
    4. Starts tools\WebhookCapture (a fake Discord) and the server with DCTEST_RUN=1.
    5. Waits for the test plugin to finish and the server to shut down, then checks
       the results, the captured webhooks, and the BepInEx log.

    Exit code is the number of failed checks.

.EXAMPLE
    .\tests\run-tests.ps1 -ServerPath E:\valheim-dev\server
    .\tests\run-tests.ps1 -ServerPath E:\valheim-dev\server -DiscordWebhook https://discord.com/api/webhooks/...
#>
param(
    [Parameter(Mandatory = $true)][string]$ServerPath,
    # Optional real Discord webhook. Every message the mod sends is also sent here so you can eyeball it.
    [string]$DiscordWebhook = $env:DCTEST_DISCORD_WEBHOOK,
    # Include slow tests (the leaderboard 5 timer test waits about a minute).
    [switch]$Slow,
    # Run with Discord embeds ("fancier messages") turned on.
    [switch]$Embeds,
    [switch]$SkipBuild,
    [int]$TimeoutMinutes = 10,
    [string]$CaptureUrl = "http://127.0.0.1:8787"
)

$ErrorActionPreference = "Stop"
$repo = Split-Path -Parent $PSScriptRoot
$outDir = Join-Path $repo "tests\out"
New-Item -ItemType Directory -Force $outDir | Out-Null
$stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$resultsPath = Join-Path $outDir "results-$stamp.json"
$serverLog = Join-Path $outDir "server-$stamp.log"
$captureLog = Join-Path $outDir "capture-$stamp.jsonl"

$failures = New-Object System.Collections.Generic.List[string]
function Fail([string]$msg) { $failures.Add($msg); Write-Host "  FAIL $msg" -ForegroundColor Red }
function Pass([string]$msg) { Write-Host "  PASS $msg" -ForegroundColor Green }

# ---- Config helper: set "Key = Value" inside [Section], creating either if missing --------------
function Set-CfgValue([string]$file, [string]$section, [string]$key, [string]$value) {
    $lines = if (Test-Path $file) { [System.Collections.Generic.List[string]](Get-Content $file) } else { New-Object System.Collections.Generic.List[string] }
    $inSection = $false; $sectionIdx = -1
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        if ($line -match '^\[(.+)\]\s*$') {
            if ($inSection) { break }
            $inSection = ($Matches[1] -eq $section)
            if ($inSection) { $sectionIdx = $i }
            continue
        }
        if ($inSection -and $line -match ('^' + [regex]::Escape($key) + '\s*=')) {
            $lines[$i] = "$key = $value"
            Set-Content -Path $file -Value $lines -Encoding utf8
            return
        }
    }
    if ($sectionIdx -lt 0) {
        $lines.Add(""); $lines.Add("[$section]"); $lines.Add("$key = $value")
    } else {
        $lines.Insert($sectionIdx + 1, "$key = $value")
    }
    Set-Content -Path $file -Value $lines -Encoding utf8
}

# ---- 1. Build ------------------------------------------------------------------------------------
if (-not $SkipBuild) {
    Write-Host "Building..." -ForegroundColor Cyan
    # -m:1 because the Jotunn publicizer step races when projects build in parallel.
    dotnet build (Join-Path $repo "DiscordConnector.sln") -c Release -m:1 -v:q -nologo | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "Build failed. Run 'dotnet build DiscordConnector.sln -c Release -m:1' to see errors." }
    dotnet build (Join-Path $repo "tools\WebhookCapture\WebhookCapture.csproj") -c Release -v:q -nologo | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "WebhookCapture build failed." }
}

# ---- 2. Install plugins ----------------------------------------------------------------------------
Get-Process valheim_server -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1
$plugins = Join-Path $ServerPath "BepInEx\plugins"
if (-not (Test-Path (Join-Path $ServerPath "BepInEx\core\BepInEx.dll"))) { throw "BepInEx is not installed in $ServerPath" }
New-Item -ItemType Directory -Force $plugins | Out-Null
Copy-Item (Join-Path $repo "DiscordConnector\bin\Release\net481\DiscordConnector.dll") $plugins -Force
Copy-Item (Join-Path $repo "DiscordConnector.Tests\bin\Release\net481\DiscordConnector.Tests.dll") $plugins -Force

# ---- 3. Config and fresh database ------------------------------------------------------------------
$cfgDir = Join-Path $ServerPath "BepInEx\config\games.nwest.valheim.discordconnector"
New-Item -ItemType Directory -Force $cfgDir | Out-Null
Remove-Item (Join-Path $cfgDir "records.db"), (Join-Path $cfgDir "records-log.db") -ErrorAction SilentlyContinue

$main = Join-Path $cfgDir "discordconnector.cfg"
Set-CfgValue $main "Main Settings" "Webhook URL" "$CaptureUrl/webhook/primary"
Set-CfgValue $main "Main Settings" "Webhook Events" "ALL"
Set-CfgValue $main "Main Settings" "Secondary Webhook URL" "$CaptureUrl/webhook/secondary"
Set-CfgValue $main "Main Settings" "Log Debug Messages" "true"
Set-CfgValue $main "Main Settings" "Use fancier discord messages" ($(if ($Embeds) { "true" } else { "false" }))

$toggles = Join-Path $cfgDir "discordconnector-toggles.cfg"
Set-CfgValue $toggles "Toggles.DebugMessages" "Debug Leaderboard Operations" "true"
Set-CfgValue $toggles "Toggles.DebugMessages" "Debug Message for Database Methods" "true"

$lb = Join-Path $cfgDir "discordconnector-leaderBoards.cfg"
foreach ($n in 1..5) {
    $s = "LeaderBoard.$n"
    Set-CfgValue $lb $s "Enabled" ($(if ($n -eq 1 -or ($n -eq 5 -and $Slow)) { "true" } else { "false" }))
    Set-CfgValue $lb $s "Leader Board Heading" "DCTEST-LB$n"
    Set-CfgValue $lb $s "Leader Board Time Range" "AllTime"
    Set-CfgValue $lb $s "Sending Period" ($(if ($n -eq 5) { "1" } else { "600" }))
    Set-CfgValue $lb $s "Type" "Descending"
    Set-CfgValue $lb $s "Number of Rankings" "3"
    Set-CfgValue $lb $s "Death Statistics" "true"
}

# ---- 4. Fake Discord ---------------------------------------------------------------------------------
# Always start a fresh copy so its log file is the one the test plugin reads.
Get-CimInstance Win32_Process -Filter "Name='dotnet.exe'" |
    Where-Object { $_.CommandLine -like "*WebhookCapture.dll*" } |
    ForEach-Object { Stop-Process -Id $_.ProcessId -Force }
Write-Host "Starting WebhookCapture..." -ForegroundColor Cyan
New-Item -ItemType File -Force $captureLog | Out-Null
$env:CAPTURE_URL = $CaptureUrl
$env:CAPTURE_LOG = $captureLog
$env:FORWARD_URL = $DiscordWebhook
$captureProc = Start-Process dotnet -ArgumentList "`"$(Join-Path $repo 'tools\WebhookCapture\bin\Release\net10.0\WebhookCapture.dll')`"" -PassThru -WindowStyle Hidden
foreach ($v in "CAPTURE_URL", "CAPTURE_LOG", "FORWARD_URL") { Remove-Item "env:$v" -ErrorAction SilentlyContinue }
$deadline = (Get-Date).AddSeconds(20)
while ($true) {
    try { Invoke-RestMethod "$CaptureUrl/health" -TimeoutSec 2 | Out-Null; break }
    catch { if ((Get-Date) -gt $deadline) { throw "WebhookCapture did not start on $CaptureUrl" }; Start-Sleep -Milliseconds 300 }
}

# ---- 5. Run the server -------------------------------------------------------------------------------
Write-Host "Starting server with tests (log: $serverLog)..." -ForegroundColor Cyan
$env:SteamAppId = "892970"
$env:DCTEST_RUN = "1"
$env:DCTEST_QUIT = "1"
$env:DCTEST_RESULTS = $resultsPath
$env:DCTEST_CAPTURE_LOG = $captureLog
$env:DCTEST_SLOW = $(if ($Slow) { "1" } else { "0" })
# Fresh world every run: a brand-new world loads through a different code path than one already saved
# in Valheim 1.0's chunked format, and the new-world path is the one that used to break.
$saveDir = Join-Path $ServerPath "dctest-saves"
Remove-Item $saveDir -Recurse -Force -ErrorAction SilentlyContinue
$server = Start-Process (Join-Path $ServerPath "valheim_server.exe") -WorkingDirectory $ServerPath -PassThru -WindowStyle Hidden `
    -RedirectStandardOutput $serverLog `
    -ArgumentList @("-nographics", "-batchmode", "-name", "DCTestServer", "-port", "2456", "-world", "DCTest",
                    "-password", "testpass123", "-public", "0", "-savedir", "`"$saveDir`"")
foreach ($v in "DCTEST_RUN", "DCTEST_QUIT", "DCTEST_RESULTS", "DCTEST_CAPTURE_LOG", "DCTEST_SLOW") { Remove-Item "env:$v" }

if (-not $server.WaitForExit($TimeoutMinutes * 60 * 1000)) {
    $server | Stop-Process -Force
    Fail "server did not finish within $TimeoutMinutes minutes (see $serverLog)"
}
Start-Sleep -Seconds 2

# ---- 6. Results --------------------------------------------------------------------------------------
Write-Host "`nIn-server tests:" -ForegroundColor Cyan
if (Test-Path $resultsPath) {
    foreach ($r in (Get-Content $resultsPath -Raw | ConvertFrom-Json)) {
        switch ($r.status) {
            "pass" { Pass $r.name }
            "skip" { Write-Host "  SKIP $($r.name) ($($r.message))" -ForegroundColor DarkGray }
            default { Fail "$($r.name): $($r.message)" }
        }
    }
} else {
    Fail "no results file; the test plugin did not finish (see $serverLog and BepInEx\LogOutput.log)"
}

Write-Host "`nServer lifecycle webhooks:" -ForegroundColor Cyan
$all = Invoke-RestMethod "$CaptureUrl/messages"
$allJson = ($all | ConvertTo-Json -Depth 20 -Compress)
foreach ($expected in "Server is starting up.", "Server has started!", "Server is stopping.", "Server has stopped!") {
    if ($allJson -like "*$expected*") { Pass "sent '$expected'" } else { Fail "never sent '$expected'" }
}
$primary = @($all | Where-Object { $_.webhook -eq "primary" }).Count
$secondary = @($all | Where-Object { $_.webhook -eq "secondary" }).Count
if ($primary -eq $secondary) { Pass "primary and secondary webhooks got the same $primary messages" }
else { Fail "primary got $primary messages but secondary got $secondary" }

Write-Host "`nBepInEx log:" -ForegroundColor Cyan
$bepLog = Join-Path $ServerPath "BepInEx\LogOutput.log"
$modErrors = Select-String -Path $bepLog -Pattern '^\[(Error|Fatal)\s*:\s*DiscordConnector' | ForEach-Object { $_.Line }
$patchErrors = Select-String -Path $bepLog -Pattern 'HarmonyLib|Failed to patch|MissingMethodException|TypeLoadException' | ForEach-Object { $_.Line }
if ($modErrors) { $modErrors | Select-Object -First 10 | ForEach-Object { Fail "mod logged: $_" } } else { Pass "no errors logged by DiscordConnector" }
if ($patchErrors) { $patchErrors | Select-Object -First 10 | ForEach-Object { Fail "patch/load problem: $_" } } else { Pass "no Harmony patch or type-load errors" }
Copy-Item $bepLog (Join-Path $outDir "LogOutput-$stamp.log")

if ($captureProc) { $captureProc | Stop-Process -Force -ErrorAction SilentlyContinue }

Write-Host ""
if ($failures.Count -eq 0) { Write-Host "ALL CHECKS PASSED" -ForegroundColor Green }
else { Write-Host "$($failures.Count) CHECK(S) FAILED" -ForegroundColor Red }
Write-Host "Results: $resultsPath"
exit $failures.Count
