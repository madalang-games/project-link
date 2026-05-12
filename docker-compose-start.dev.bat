@echo off
setlocal

cd /d "%~dp0"
call "%~dp0docker-compose.dev.bat" %*
exit /b %ERRORLEVEL%
