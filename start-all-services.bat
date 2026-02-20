@echo off
echo Starting all RankUpAPI microservices...
echo.

echo Starting ExamService...
start "ExamService" cmd /k "cd /d Services\ExamService\ExamService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting UserService...
start "UserService" cmd /k "cd /d Services\UserService\UserService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting AdminService...
start "AdminService" cmd /k "cd /d Services\AdminService\AdminService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting QuestionService...
start "QuestionService" cmd /k "cd /d Services\QuestionService\QuestionService.API && dotnet run"

timeout /t 3 /nobreak >nul

@REM echo Starting QuizService...
@REM start "QuizService" cmd /k "cd /d Services\QuizService\QuizService.API && dotnet run"

@REM timeout /t 3 /nobreak >nul

@REM echo Starting PaymentService...
@REM start "PaymentService" cmd /k "cd /d Services\PaymentService\PaymentService.API && dotnet run"

@REM timeout /t 3 /nobreak >nul

echo Starting SubscriptionService...
start "SubscriptionService" cmd /k "cd /d Services\SubscriptionService\SubscriptionService.API && dotnet run"

timeout /t 3 /nobreak >nul

@REM echo Starting HomeDashboardService...
@REM start "HomeDashboardService" cmd /k "cd /d Services\HomeDashboardService\HomeDashboardService.API && dotnet run"

@REM timeout /t 3 /nobreak >nul

echo Starting MasterService...
start "MasterService" cmd /k "cd /d Services\MasterService\MasterService.API && dotnet run"

timeout /t 3 /nobreak >nul

@REM echo Starting TestService...
@REM start "TestService" cmd /k "cd /d Services\TestService\TestService.API && dotnet run"

@REM timeout /t 3 /nobreak >nul

echo All services started! Check individual windows for status.
echo.
echo Service URLs:
echo ExamService: http://0.0.0.0:5000/swagger
echo UserService: http://0.0.0.0:5002/swagger
echo AdminService: http://0.0.0.0:56924/swagger
echo QuestionService: http://0.0.0.0:56922/swagger
echo QuizService: http://0.0.0.0:56921/swagger
echo PaymentService: http://0.0.0.0:56920/swagger
echo SubscriptionService: http://0.0.0.0:56926/swagger
echo HomeDashboardService: http://0.0.0.0:56928/swagger
echo MasterService: http://0.0.0.0:5009/swagger
echo TestService: http://0.0.0.0:7001/swagger
echo.
echo Mobile App Access URLs:
echo TestService API: http://0.0.0.0:7001/api
echo Gateway API: http://0.0.0.0:8000/api
echo.
echo Local Development URLs:
echo Gateway API: http://localhost:8000/api
echo All services accessible via localhost or 0.0.0.0 on their respective ports
echo.
pause
