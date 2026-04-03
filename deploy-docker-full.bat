@echo off
echo ========================================
echo RankUp API Full Docker Deployment
echo ========================================
echo.

echo Step 1: Copy database script to container...
docker cp database/RankUp_UserDB_Script.sql rankup-sqlserver:/tmp/database_script.sql
if errorlevel 1 (
    echo ERROR: Failed to copy database script
    echo Make sure SQL Server container is running first
    pause
    exit /b 1
)
echo Database script copied successfully
echo.

echo Step 2: Deploy database...
call deploy-database.bat
if errorlevel 1 (
    echo ERROR: Database deployment failed
    pause
    exit /b 1
)
echo.

echo Step 3: Starting all services...
docker compose -f docker/docker-compose.yml up -d
if errorlevel 1 (
    echo ERROR: Failed to start services
    pause
    exit /b 1
)
echo All services started successfully
echo.

echo Step 4: Waiting for services to initialize...
timeout /t 60 /nobreak >nul
echo.

echo Step 5: Checking service status...
docker compose -f docker/docker-compose.yml ps
echo.

echo ========================================
echo Deployment completed successfully!
echo ========================================
echo.
echo API Gateway: http://localhost:5087
echo SQL Server: localhost:1433
echo.
echo Services running:
echo - Gateway API (Port 5087)
echo - User Service
echo - Exam Service
echo - Admin Service
echo - Master Service
echo - Question Service
echo - Quiz Service
echo - Subscription Service
echo - Payment Service
echo - Home Dashboard Service
echo - Test Service
echo.
pause
