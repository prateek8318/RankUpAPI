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

echo Starting QuizService...
start "QuizService" cmd /k "cd /d Services\QuizService\QuizService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting PaymentService...
start "PaymentService" cmd /k "cd /d Services\PaymentService\PaymentService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting SubscriptionService...
start "SubscriptionService" cmd /k "cd /d Services\SubscriptionService\SubscriptionService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo All services started! Check individual windows for status.
echo.
echo Service URLs:
echo ExamService: http://localhost:5000/swagger
echo UserService: http://localhost:5002/swagger
echo AdminService: http://localhost:56924/swagger
echo QuestionService: http://localhost:56922/swagger
echo QuizService: http://localhost:56921/swagger
echo PaymentService: http://localhost:56920/swagger
echo SubscriptionService: http://localhost:56926/swagger
echo.
pause
