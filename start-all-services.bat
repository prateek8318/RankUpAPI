@echo off
echo Starting all RankUpAPI microservices...
echo.

echo Starting MasterService...
start "MasterService" cmd /k "cd /d Services\MasterService\MasterService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting ExamService...
start "ExamService" cmd /k "cd /d Services\ExamService\ExamService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting UserService...
start "UserService" cmd /k "cd /d Services\UserService\UserService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting AdminService...
start "AdminService" cmd /k "cd /d Services\AdminService\AdminService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting SubscriptionService...
start "SubscriptionService" cmd /k "cd /d Services\SubscriptionService\SubscriptionService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting QuestionService...
start "QuestionService" cmd /k "cd /d Services\QuestionService\QuestionService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting TestService...
start "TestService" cmd /k "cd /d Services\TestService\TestService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting QuizService...
start "QuizService" cmd /k "cd /d Services\QuizService\QuizService.API && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting API Gateway...
start "APIGateway" cmd /k "cd /d Services\GatewayAPI && dotnet run"

timeout /t 3 /nobreak >nul

echo All services started! Check individual windows for status.
echo.
echo ========================================
echo SERVICE URLs (Swagger Documentation):
echo ========================================
echo MasterService: http://localhost:5009/swagger
echo ExamService: http://localhost:5000/swagger
echo UserService: http://localhost:5002/swagger
echo AdminService: http://localhost:56923/swagger
echo SubscriptionService: http://localhost:56925/swagger
echo QuestionService: http://localhost:56916/swagger
echo TestService: http://localhost:56917/swagger
echo QuizService: http://localhost:56918/swagger
echo API Gateway: http://localhost:5001/swagger
echo.
echo ========================================
echo MOBILE APP ACCESS URLS:
echo ========================================
echo API Gateway: http://localhost:5001/api
echo Direct Services (if needed):
echo - TestService: http://localhost:56917/api
echo - QuestionService: http://localhost:56916/api
echo - SubscriptionService: http://localhost:56925/api
echo - UserService: http://localhost:5002/api
echo.
echo ========================================
echo IMPORTANT ENDPOINTS:
echo ========================================
echo Subscription Flow:
echo - Create Order: POST /api/subscription/payments/create-order
echo - Verify Payment: POST /api/subscription/payments/verify
echo - Active Subscriptions: GET /api/subscription/user/active
echo.
echo Question Management:
echo - Get Questions: GET /api/questions/paged
echo - Create Question: POST /api/questions
echo - Statistics: GET /api/questions/statistics
echo.
echo Test Management:
echo - Start Test: POST /api/tests/{id}/start
echo - Submit Answer: POST /api/tests/submit-answer
echo - Complete Test: POST /api/tests/complete
echo - Test History: GET /api/tests/history
echo.
echo ========================================
echo DATABASE CONNECTIONS:
echo ========================================
echo RankUp_SubscriptionDB: Subscription & Payments
echo RankUp_QuestionDB: Questions & Topics
echo RankUp_TestDB: Tests & Attempts
echo RankUp_MasterDB: Exams, Subjects, Qualifications
echo.
echo ========================================
echo RAZORPAY CONFIGURATION:
echo ========================================
echo Key ID: rzp_test_hCRLFPf6rY3elm
echo Key Secret: m1lFhxsJTlb78bz2owxRy0E8
echo Webhook: /api/webhooks/razorpay
echo.
pause
