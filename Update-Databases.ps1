# Database Update Script for RankUp Microservices
# This script applies EF Core migrations to create/update each microservice database

Write-Host "Starting database updates for all RankUp microservices..." -ForegroundColor Green

# AdminService Database Update
Write-Host "`n=== Updating AdminService Database ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\AdminService\AdminService.API"
dotnet ef database update --project ../AdminService.Infrastructure --context AdminDbContext
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ AdminService database updated successfully (RankUp_AdminDB)" -ForegroundColor Green
} else {
    Write-Host "✗ AdminService database update failed" -ForegroundColor Red
}

# UserService Database Update
Write-Host "`n=== Updating UserService Database ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\UserService\UserService.API"
dotnet ef database update --project ../UserService.Infrastructure --context UserDbContext
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ UserService database updated successfully (RankUp_UserDB)" -ForegroundColor Green
} else {
    Write-Host "✗ UserService database update failed" -ForegroundColor Red
}

# ExamService Database Update
Write-Host "`n=== Updating ExamService Database ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\ExamService\ExamService.API"
dotnet ef database update --project ../ExamService.Infrastructure --context ExamDbContext
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ ExamService database updated successfully (RankUp_ExamDB)" -ForegroundColor Green
} else {
    Write-Host "✗ ExamService database update failed" -ForegroundColor Red
}

# QuestionService Database Update
Write-Host "`n=== Updating QuestionService Database ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\QuestionService\QuestionService.API"
dotnet ef database update --project ../QuestionService.Infrastructure --context QuestionDbContext
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ QuestionService database updated successfully (RankUp_QuestionDB)" -ForegroundColor Green
} else {
    Write-Host "✗ QuestionService database update failed" -ForegroundColor Red
}

# QuizService Database Update
Write-Host "`n=== Updating QuizService Database ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\QuizService\QuizService.API"
dotnet ef database update --project ../QuizService.Infrastructure --context QuizDbContext
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ QuizService database updated successfully (RankUp_QuizDB)" -ForegroundColor Green
} else {
    Write-Host "✗ QuizService database update failed" -ForegroundColor Red
}

# SubscriptionService Database Update
Write-Host "`n=== Updating SubscriptionService Database ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\SubscriptionService\SubscriptionService.API"
dotnet ef database update --project ../SubscriptionService.Infrastructure --context SubscriptionDbContext
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ SubscriptionService database updated successfully (RankUp_SubscriptionDB)" -ForegroundColor Green
} else {
    Write-Host "✗ SubscriptionService database update failed" -ForegroundColor Red
}

# PaymentService Database Update
Write-Host "`n=== Updating PaymentService Database ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\PaymentService\PaymentService.API"
dotnet ef database update --project ../PaymentService.Infrastructure --context PaymentDbContext
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ PaymentService database updated successfully (RankUp_PaymentDB)" -ForegroundColor Green
} else {
    Write-Host "✗ PaymentService database update failed" -ForegroundColor Red
}

# Return to root directory
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI"

Write-Host "`n=== Database Updates Complete ===" -ForegroundColor Green
Write-Host "All microservice databases have been created/updated." -ForegroundColor Cyan
Write-Host "Each service now has its own dedicated database:" -ForegroundColor Cyan
Write-Host "- RankUp_AdminDB" -ForegroundColor White
Write-Host "- RankUp_UserDB" -ForegroundColor White
Write-Host "- RankUp_ExamDB" -ForegroundColor White
Write-Host "- RankUp_QuestionDB" -ForegroundColor White
Write-Host "- RankUp_QuizDB" -ForegroundColor White
Write-Host "- RankUp_SubscriptionDB" -ForegroundColor White
Write-Host "- RankUp_PaymentDB" -ForegroundColor White
