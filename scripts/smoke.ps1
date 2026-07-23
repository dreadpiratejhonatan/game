# Smoke check headless (mods + conteúdo mínimo)
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot\..
dotnet run -- --smoke
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host "smoke.ps1: OK"
