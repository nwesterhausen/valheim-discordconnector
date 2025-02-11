param ($manifestFile, $versionString)

try {
    $manifest = Get-Content $manifestFile
    $manifest = $manifest -replace '"version_number":\s*"([^"]*)"', "`"version_number`": `"$versionString`""
    Set-Content -Path $manifestFile -Value $manifest
} catch {
    Write-Error $_.Exception.Message
}