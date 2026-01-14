@echo off
echo Starting migration generation for all RankUp microservices...

echo.
echo === Generating AdminService Migrations ===
cd "Services\AdminService\AdminService.API"
dotnet ef migrations add InitialCreate --project ../AdminService.Infrastructure --context AdminDbContext --startup-project .
if %errorlevel% equ 0 (
    echo ✓ AdminService migrations generated successfully
) else (
    echo ✗ AdminService migration generation failed
)

echo.
echo === Generating UserService Migrations ===
cd "../../UserService/UserService.API"
dotnet ef migrations add InitialCreate --project ../UserService.Infrastructure --context UserDbContext --startup-project .
if %errorlevel% equ 0 (
    echo ✓ UserService migrations generated successfully
) else (
    echo ✗ UserService migration generation failed
)

echo.
echo === Generating ExamService Migrations ===
cd "../../ExamService/ExamService.API"
dotnet ef migrations add InitialCreate --project ../ExamService.Infrastructure --context ExamDbContext --startup-project .
if %errorlevel% equ 0 (
    echo ✓ ExamService migrations generated successfully
) else (
    echo ✗ ExamService migration generation failed
)

echo.
echo === Generating QuestionService Migrations ===
cd "../../QuestionService/QuestionService.API"
dotnet ef migrations add InitialCreate --project ../QuestionService.Infrastructure --context QuestionDbContext --startup-project .
if %errorlevel% equ 0 (
    echo ✓ QuestionService migrations generated successfully
) else (
    echo ✗ QuestionService migration generation failed
)

echo.
echo === Generating QuizService Migrations ===
cd "../../QuizService/QuizService.API"
dotnet ef migrations add InitialCreate --project ../QuizService.Infrastructure --context QuizDbContext --startup-project .
if %errorlevel% equ 0 (
    echo ✓ QuizService migrations generated successfully
) else (
    echo ✗ QuizService migration generation failed
)

echo.
echo === Generating SubscriptionService Migrations ===
cd "../../SubscriptionService/SubscriptionService.API"
dotnet ef migrations add InitialCreate --project ../SubscriptionService.Infrastructure --context SubscriptionDbContext --startup-project .
if %errorlevel% equ 0 (
    echo ✓ SubscriptionService migrations generated successfully
) else (
    echo ✗ SubscriptionService migration generation failed
)

echo.
echo === Generating PaymentService Migrations ===
cd "../../PaymentService/PaymentService.API"
dotnet ef migrations add InitialCreate --project ../PaymentService.Infrastructure --context PaymentDbContext --startup-project .
if %errorlevel% equ 0 (
    echo ✓ PaymentService migrations generated successfully
) else (
    echo ✗ PaymentService migration generation failed
)

cd ../../..
echo.
echo === Migration Generation Complete ===
echo All migrations have been generated for each microservice database.
echo Databases will be created when you run each service.

pause
