@echo off
REM RankUpAPI - Start Docker stack (run from repo root)
REM Requires: Docker Desktop installed and running

cd /d "%~dp0.."

if not exist "docker\.env" (
  copy docker\.env.example docker\.env
)

echo Starting RankUpAPI stack...
docker compose -f docker\docker-compose.yml --env-file docker\.env up -d --build

echo.
echo Gateway: http://localhost:5087
echo SQL Server: localhost:1433 (sa / password in docker\.env)
echo.
pause
