@echo off
title Iniciar Full Stack - Technical Test
color 0A

echo ========================================
echo     INICIANDO APLICACION FULL STACK
echo ========================================
echo.

echo [1/2] Iniciando Backend API...
start "Backend API" cmd /k "cd /d \"%~dp0TechnicalTest.Backend\" && echo Backend iniciando en http://localhost:5110 && dotnet run"

echo Esperando a que el backend inicie...
timeout /t 5 /nobreak >nul

echo.
echo [2/2] Iniciando Frontend Blazor...
start "Frontend Blazor" cmd /k "cd /d \"%~dp0TechnicalTest.Frontend\" && echo Frontend iniciando en https://localhost:7004 && dotnet run"

echo.
echo ========================================
echo     AMBOS PROYECTOS INICIADOS
echo ========================================
echo.
echo Backend:  http://localhost:5110/swagger
echo Frontend: https://localhost:7004
echo.
echo Presiona cualquier tecla para salir...
pause >nul
