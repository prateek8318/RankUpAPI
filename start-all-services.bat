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

@REM echo Starting SubscriptionService...
@REM start "SubscriptionService" cmd /k "cd /d Services\SubscriptionService\SubscriptionService.API && dotnet run"

@REM timeout /t 3 /nobreak >nul

echo Starting HomeDashboardService...
start "HomeDashboardService" cmd /k "cd /d Services\HomeDashboardService\HomeDashboardService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting MasterService...
start "MasterService" cmd /k "cd /d Services\MasterService\MasterService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting TestService...
start "TestService" cmd /k "cd /d Services\TestService\TestService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting QualificationService...
start "QualificationService" cmd /k "cd /d Services\QualificationService\QualificationService.API && dotnet run"

timeout /t 3 /nobreak >nul 

echo All services started! Check individual windows for status.
echo.
echo Service URLs:
echo ExamService: http://192.168.1.14:5000/swagger
echo UserService: http://192.168.1.14:5002/swagger
echo AdminService: http://192.168.1.14:56924/swagger
echo QuestionService: http://192.168.1.14:56922/swagger
echo QuizService: http://192.168.1.14:56921/swagger
echo PaymentService: http://192.168.1.14:56920/swagger
echo SubscriptionService: http://192.168.1.14:56926/swagger
echo HomeDashboardService: http://192.168.1.14:56928/swagger
echo MasterService: http://192.168.1.14:5009/swagger
echo QualificationService: http://192.168.1.14:5011/swagger
echo TestService: http://192.168.1.14:7001/swagger
echo.
echo Mobile App Access URLs:
echo TestService API: http://192.168.1.14:7001/api
echo Gateway API: http://192.168.1.14:7000/api
echo.
pause
