@echo off
setlocal

set "PROJECT_WIN=%~dp0"
for %%I in ("%PROJECT_WIN%.") do set "PROJECT_WIN=%%~fI"

for /f "delims=" %%P in ('wsl.exe wslpath -a "%PROJECT_WIN%" 2^>nul') do set "PROJECT_WSL=%%P"

if not defined PROJECT_WSL (
    echo Failed to resolve WSL path for "%PROJECT_WIN%".
    echo Make sure WSL is installed and available from Windows.
    pause
    exit /b 1
)

start "project-link WSL" wsl.exe --cd "%PROJECT_WSL%"

endlocal
