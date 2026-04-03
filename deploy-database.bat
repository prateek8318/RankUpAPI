@echo off
echo ========================================
echo RankUp User Database Deployment Script
echo ========================================
echo.

echo Step 1: Starting SQL Server container...
docker compose -f docker/docker-compose.yml up -d sqlserver
if errorlevel 1 (
    echo ERROR: Failed to start SQL Server container
    pause
    exit /b 1
)
echo SQL Server container started successfully
echo.

echo Step 2: Waiting for SQL Server to be ready...
timeout /t 30 /nobreak >nul
echo.

echo Step 3: Deploying database schema and data...
docker exec rankup-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "RankUp@ProdPass2026!" -i /tmp/database_script.sql
if errorlevel 1 (
    echo ERROR: Database deployment failed
    pause
    exit /b 1
)
echo Database deployed successfully!
echo.

echo Step 4: Verifying database deployment...
docker exec rankup-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "RankUp@ProdPass2026!" -Q "SELECT name FROM sys.databases WHERE name = 'RankUp_UserDB'"
echo.

echo ========================================
echo Database deployment completed!
echo ========================================
echo.
echo Database: RankUp_UserDB
echo Server: localhost:1433
echo User: sa
echo Password: RankUp@ProdPass2026!
echo.
pause
