param ($tomlFile, $versionString)

try
{
    $tomldata = Get-Content $tomlFile
    $tomldata = $tomldata -replace 'versionNumber = "([0-9.]*)"', "versionNumber = `"$versionString`""
    Set-Content -Path $tomlFile -Value $tomldata
}
catch
{
    Write-Error $_.Exception.Message
}