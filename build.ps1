#!/usr/bin/env pwsh
Set-Location "d:\Projects\AksharaShift\AksharaShift"
dotnet build -c Release
Write-Host "Build complete" -ForegroundColor Green
