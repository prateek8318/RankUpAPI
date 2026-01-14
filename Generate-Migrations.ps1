# Migration Generation Script for RankUp Microservices
# This script generates EF Core migrations for each microservice

Write-Host "Starting migration generation for all RankUp microservices..." -ForegroundColor Green

# AdminService Migrations
Write-Host "`n=== Generating AdminService Migrations ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\AdminService\AdminService.API"
dotnet ef migrations add InitialCreate --project ../AdminService.Infrastructure --context AdminDbContext --startup-project . --output-dir Migrations
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ AdminService migrations generated successfully" -ForegroundColor Green
} else {
    Write-Host "✗ AdminService migration generation failed" -ForegroundColor Red
}

# UserService Migrations
Write-Host "`n=== Generating UserService Migrations ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\UserService\UserService.API"
dotnet ef migrations add InitialCreate --project ../UserService.Infrastructure --context UserDbContext --startup-project . --output-dir Migrations
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ UserService migrations generated successfully" -ForegroundColor Green
} else {
    Write-Host "✗ UserService migration generation failed" -ForegroundColor Red
}

# ExamService Migrations
Write-Host "`n=== Generating ExamService Migrations ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\ExamService\ExamService.API"
dotnet ef migrations add InitialCreate --project ../ExamService.Infrastructure --context ExamDbContext --startup-project . --output-dir Migrations
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ ExamService migrations generated successfully" -ForegroundColor Green
} else {
    Write-Host "✗ ExamService migration generation failed" -ForegroundColor Red
}

# QuestionService Migrations
Write-Host "`n=== Generating QuestionService Migrations ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\QuestionService\QuestionService.API"
dotnet ef migrations add InitialCreate --project ../QuestionService.Infrastructure --context QuestionDbContext --startup-project . --output-dir Migrations
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ QuestionService migrations generated successfully" -ForegroundColor Green
} else {
    Write-Host "✗ QuestionService migration generation failed" -ForegroundColor Red
}

# QuizService Migrations
Write-Host "`n=== Generating QuizService Migrations ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\QuizService\QuizService.API"
dotnet ef migrations add InitialCreate --project ../QuizService.Infrastructure --context QuizDbContext --startup-project . --output-dir Migrations
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ QuizService migrations generated successfully" -ForegroundColor Green
} else {
    Write-Host "✗ QuizService migration generation failed" -ForegroundColor Red
}

# SubscriptionService Migrations
Write-Host "`n=== Generating SubscriptionService Migrations ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\SubscriptionService\SubscriptionService.API"
dotnet ef migrations add InitialCreate --project ../SubscriptionService.Infrastructure --context SubscriptionDbContext --startup-project . --output-dir Migrations
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ SubscriptionService migrations generated successfully" -ForegroundColor Green
} else {
    Write-Host "✗ SubscriptionService migration generation failed" -ForegroundColor Red
}

# PaymentService Migrations
Write-Host "`n=== Generating PaymentService Migrations ===" -ForegroundColor Yellow
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI\Services\PaymentService\PaymentService.API"
dotnet ef migrations add InitialCreate --project ../PaymentService.Infrastructure --context PaymentDbContext --startup-project . --output-dir Migrations
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ PaymentService migrations generated successfully" -ForegroundColor Green
} else {
    Write-Host "✗ PaymentService migration generation failed" -ForegroundColor Red
}

# Return to root directory
Set-Location "c:\Users\abhij\CascadeProjects\RankUpAPI"

Write-Host "`n=== Migration Generation Complete ===" -ForegroundColor Green
Write-Host "All migrations have been generated for each microservice database." -ForegroundColor Cyan
Write-Host "Databases will be created when you run each service." -ForegroundColor Cyan

Write-Host "`n=== Database Update Commands ===" -ForegroundColor Yellow
Write-Host "To apply migrations to databases, run these commands individually:" -ForegroundColor White
Write-Host "AdminService: cd Services/AdminService/AdminService.API; dotnet ef database update --project ../AdminService.Infrastructure" -ForegroundColor Gray
Write-Host "UserService: cd Services/UserService/UserService.API; dotnet ef database update --project ../UserService.Infrastructure" -ForegroundColor Gray
Write-Host "ExamService: cd Services/ExamService/ExamService.API; dotnet ef database update --project ../ExamService.Infrastructure" -ForegroundColor Gray
Write-Host "QuestionService: cd Services/QuestionService/QuestionService.API; dotnet ef database update --project ../QuestionService.Infrastructure" -ForegroundColor Gray
Write-Host "QuizService: cd Services/QuizService/QuizService.API; dotnet ef database update --project ../QuizService.Infrastructure" -ForegroundColor Gray
Write-Host "SubscriptionService: cd Services/SubscriptionService/SubscriptionService.API; dotnet ef database update --project ../SubscriptionService.Infrastructure" -ForegroundColor Gray
Write-Host "PaymentService: cd Services/PaymentService/PaymentService.API; dotnet ef database update --project ../PaymentService.Infrastructure" -ForegroundColor Gray
