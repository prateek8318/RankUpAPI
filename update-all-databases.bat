@echo off
echo Updating all microservice databases...

echo.
echo === Updating QuestionService Database ===
cd "Services\QuestionService\QuestionService.API"
dotnet ef database update --project ../QuestionService.Infrastructure --context QuestionDbContext
if %errorlevel% equ 0 (
    echo ✓ QuestionService database updated successfully
) else (
    echo ✗ QuestionService database update failed
)

echo.
echo === Updating QuizService Database ===
cd "../../QuizService/QuizService.API"
dotnet ef database update --project ../QuizService.Infrastructure --context QuizDbContext
if %errorlevel% equ 0 (
    echo ✓ QuizService database updated successfully
) else (
    echo ✗ QuizService database update failed
)

echo.
echo === Updating SubscriptionService Database ===
cd "../../SubscriptionService/SubscriptionService.API"
dotnet ef database update --project ../SubscriptionService.Infrastructure --context SubscriptionDbContext
if %errorlevel% equ 0 (
    echo ✓ SubscriptionService database updated successfully
) else (
    echo ✗ SubscriptionService database update failed
)

echo.
echo === Updating PaymentService Database ===
cd "../../PaymentService/PaymentService.API"
dotnet ef database update --project ../PaymentService.Infrastructure --context PaymentDbContext
if %errorlevel% equ 0 (
    echo ✓ PaymentService database updated successfully
) else (
    echo ✗ PaymentService database update failed
)

cd ../../..
echo.
echo === Database Updates Complete ===
echo All microservice databases have been created/updated.

pause
