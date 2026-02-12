@echo off
REM Update database using EF Core migrations.
REM Run from solution root: Templates\BonyanTemplate.Infrastructure\Scripts\Update-Database.cmd

REM Ensure dotnet global tools are on PATH (required after .NET 10 install if ef is not found)
if exist "%USERPROFILE%\.dotnet\tools" set "PATH=%USERPROFILE%\.dotnet\tools;%PATH%"

cd /d "%~dp0..\..\.."
dotnet ef database update --project Templates\BonyanTemplate.Infrastructure --startup-project Templates\BonyanTemplate.Mvc --context BonyanTemplateDbContext
exit /b %ERRORLEVEL%
