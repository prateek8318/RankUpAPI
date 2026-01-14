#!/bin/bash

# Migration Generation Script for RankUp Microservices (Bash Version)
echo "Starting migration generation for all RankUp microservices..."

# AdminService Migrations
echo -e "\n=== Generating AdminService Migrations ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/AdminService/AdminService.API"
dotnet ef migrations add InitialCreate --project ../AdminService.Infrastructure --context AdminDbContext --startup-project . --output-dir Migrations
if [ $? -eq 0 ]; then
    echo "✓ AdminService migrations generated successfully"
else
    echo "✗ AdminService migration generation failed"
fi

# UserService Migrations
echo -e "\n=== Generating UserService Migrations ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/UserService/UserService.API"
dotnet ef migrations add InitialCreate --project ../UserService.Infrastructure --context UserDbContext --startup-project . --output-dir Migrations
if [ $? -eq 0 ]; then
    echo "✓ UserService migrations generated successfully"
else
    echo "✗ UserService migration generation failed"
fi

# ExamService Migrations
echo -e "\n=== Generating ExamService Migrations ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/ExamService/ExamService.API"
dotnet ef migrations add InitialCreate --project ../ExamService.Infrastructure --context ExamDbContext --startup-project . --output-dir Migrations
if [ $? -eq 0 ]; then
    echo "✓ ExamService migrations generated successfully"
else
    echo "✗ ExamService migration generation failed"
fi

# QuestionService Migrations
echo -e "\n=== Generating QuestionService Migrations ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/QuestionService/QuestionService.API"
dotnet ef migrations add InitialCreate --project ../QuestionService.Infrastructure --context QuestionDbContext --startup-project . --output-dir Migrations
if [ $? -eq 0 ]; then
    echo "✓ QuestionService migrations generated successfully"
else
    echo "✗ QuestionService migration generation failed"
fi

# QuizService Migrations
echo -e "\n=== Generating QuizService Migrations ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/QuizService/QuizService.API"
dotnet ef migrations add InitialCreate --project ../QuizService.Infrastructure --context QuizDbContext --startup-project . --output-dir Migrations
if [ $? -eq 0 ]; then
    echo "✓ QuizService migrations generated successfully"
else
    echo "✗ QuizService migration generation failed"
fi

# SubscriptionService Migrations
echo -e "\n=== Generating SubscriptionService Migrations ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/SubscriptionService/SubscriptionService.API"
dotnet ef migrations add InitialCreate --project ../SubscriptionService.Infrastructure --context SubscriptionDbContext --startup-project . --output-dir Migrations
if [ $? -eq 0 ]; then
    echo "✓ SubscriptionService migrations generated successfully"
else
    echo "✗ SubscriptionService migration generation failed"
fi

# PaymentService Migrations
echo -e "\n=== Generating PaymentService Migrations ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/PaymentService/PaymentService.API"
dotnet ef migrations add InitialCreate --project ../PaymentService.Infrastructure --context PaymentDbContext --startup-project . --output-dir Migrations
if [ $? -eq 0 ]; then
    echo "✓ PaymentService migrations generated successfully"
else
    echo "✗ PaymentService migration generation failed"
fi

# Return to root directory
cd "c:/Users/abhij/CascadeProjects/RankUpAPI"

echo -e "\n=== Migration Generation Complete ==="
echo "All migrations have been generated for each microservice database."
echo "Databases will be created when you run each service."

echo -e "\n=== Database Update Commands ==="
echo "To apply migrations to databases, run these commands individually:"
echo "AdminService: cd Services/AdminService/AdminService.API && dotnet ef database update --project ../AdminService.Infrastructure"
echo "UserService: cd Services/UserService/UserService.API && dotnet ef database update --project ../UserService.Infrastructure"
echo "ExamService: cd Services/ExamService/ExamService.API && dotnet ef database update --project ../ExamService.Infrastructure"
echo "QuestionService: cd Services/QuestionService/QuestionService.API && dotnet ef database update --project ../QuestionService.Infrastructure"
echo "QuizService: cd Services/QuizService/QuizService.API && dotnet ef database update --project ../QuizService.Infrastructure"
echo "SubscriptionService: cd Services/SubscriptionService/SubscriptionService.API && dotnet ef database update --project ../SubscriptionService.Infrastructure"
echo "PaymentService: cd Services/PaymentService/PaymentService.API && dotnet ef database update --project ../PaymentService.Infrastructure"
