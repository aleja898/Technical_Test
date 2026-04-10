# Script para iniciar Full Stack - Technical Test
Write-Host "========================================" -ForegroundColor Green
Write-Host "    INICIANDO APLICACION FULL STACK" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Ruta base del proyecto
$basePath = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "[1/2] Iniciando Backend API..." -ForegroundColor Yellow
$backendJob = Start-Job -ScriptBlock {
    param($path)
    Set-Location $path
    Write-Host "Backend iniciando en http://localhost:5110" -ForegroundColor Cyan
    dotnet run
} -Argument "$basePath\TechnicalTest.Backend"

Write-Host "Esperando a que el backend inicie..." -ForegroundColor Gray
Start-Sleep -Seconds 5

Write-Host ""
Write-Host "[2/2] Iniciando Frontend Blazor..." -ForegroundColor Yellow
$frontendJob = Start-Job -ScriptBlock {
    param($path)
    Set-Location $path
    Write-Host "Frontend iniciando en https://localhost:7004" -ForegroundColor Cyan
    dotnet run
} -Argument "$basePath\TechnicalTest.Frontend"

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "     AMBOS PROYECTOS INICIADOS" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Backend:  http://localhost:5110/swagger" -ForegroundColor White
Write-Host "Frontend: https://localhost:7004" -ForegroundColor White
Write-Host ""
Write-Host "Presiona Ctrl+C para detener ambos proyectos" -ForegroundColor Gray

# Mantener el script corriendo
try {
    while ($true) {
        Start-Sleep -Seconds 1
    }
}
finally {
    Write-Host "Deteniendo todos los proyectos..." -ForegroundColor Red
    Stop-Job $backendJob -ErrorAction SilentlyContinue
    Stop-Job $frontendJob -ErrorAction SilentlyContinue
    Remove-Job $backendJob -ErrorAction SilentlyContinue
    Remove-Job $frontendJob -ErrorAction SilentlyContinue
    Write-Host "Proyectos detenidos." -ForegroundColor Red
}
