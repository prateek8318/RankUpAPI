@echo off
echo ========================================
echo Starting All Services with Gateway
echo ========================================
echo.

echo [1/8] Starting Gateway API...
cd Services\GatewayAPI
start "Gateway API" cmd /k "dotnet run"
timeout /t 3 >nul

echo [2/8] Starting UserService...
cd Services\UserService\UserService.API
start "UserService" cmd /k "dotnet run"
timeout /t 3 >nul

echo [3/8] Starting ExamService...
cd Services\ExamService\ExamService.API
start "ExamService" cmd /k "dotnet run"
timeout /t 3 >nul

echo [4/8] Starting AdminService...
cd Services\AdminService\AdminService.API
start "AdminService" cmd /k "dotnet run"
timeout /t 3 >nul

echo [5/8] Starting MasterService...
cd Services\MasterService\MasterService.API
start "MasterService" cmd /k "dotnet run"
timeout /t 3 >nul

echo [6/8] Starting HomeDashboardService...
cd Services\HomeDashboardService\HomeDashboardService.API
start "HomeDashboard" cmd /k "dotnet run"
timeout /t 3 >nul

echo [7/8] Starting Other Services...
cd Services\PaymentService\PaymentService.API
start "PaymentService" cmd /k "dotnet run"
timeout /t 2 >nul

cd Services\QuizService\QuizService.API
start "QuizService" cmd /k "dotnet run"
timeout /t 2 >nul

cd Services\SubscriptionService\SubscriptionService.API
start "SubscriptionService" cmd /k "dotnet run"
timeout /t 2 >nul

cd Services\QuestionService\QuestionService.API
start "QuestionService" cmd /k "dotnet run"
timeout /t 2 >nul

cd Services\QualificationService\QualificationService.API
start "QualificationService" cmd /k "dotnet run"

echo.
echo ========================================
echo All Services Started!
echo ========================================
echo.
echo Gateway Access:
echo   Local: http://localhost:8000
echo   Network: http://192.168.1.14:8000
echo.
echo Gateway Routes:
echo   - Exam Service: /api/exam/* or /api/exams/*
echo   - User Service: /api/users/*
echo   - Admin Service: /api/admin/*
echo   - Master Service: /api/master/* or /api/languages/*
echo   - Home Dashboard: /api/homedashboard/*
echo.
echo All services are running with IP address support!
echo.
pause
