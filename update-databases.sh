#!/bin/bash

# Database Update Script for RankUp Microservices (Bash Version)
echo "Starting database updates for all RankUp microservices..."

# AdminService Database Update
echo -e "\n=== Updating AdminService Database ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/AdminService/AdminService.API"
dotnet ef database update --project ../AdminService.Infrastructure --context AdminDbContext
if [ $? -eq 0 ]; then
    echo "✓ AdminService database updated successfully (RankUp_AdminDB)"
else
    echo "✗ AdminService database update failed"
fi

# UserService Database Update
echo -e "\n=== Updating UserService Database ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/UserService/UserService.API"
dotnet ef database update --project ../UserService.Infrastructure --context UserDbContext
if [ $? -eq 0 ]; then
    echo "✓ UserService database updated successfully (RankUp_UserDB)"
else
    echo "✗ UserService database update failed"
fi

# ExamService Database Update
echo -e "\n=== Updating ExamService Database ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/ExamService/ExamService.API"
dotnet ef database update --project ../ExamService.Infrastructure --context ExamDbContext
if [ $? -eq 0 ]; then
    echo "✓ ExamService database updated successfully (RankUp_ExamDB)"
else
    echo "✗ ExamService database update failed"
fi

# QuestionService Database Update
echo -e "\n=== Updating QuestionService Database ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/QuestionService/QuestionService.API"
dotnet ef database update --project ../QuestionService.Infrastructure --context QuestionDbContext
if [ $? -eq 0 ]; then
    echo "✓ QuestionService database updated successfully (RankUp_QuestionDB)"
else
    echo "✗ QuestionService database update failed"
fi

# QuizService Database Update
echo -e "\n=== Updating QuizService Database ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/QuizService/QuizService.API"
dotnet ef database update --project ../QuizService.Infrastructure --context QuizDbContext
if [ $? -eq 0 ]; then
    echo "✓ QuizService database updated successfully (RankUp_QuizDB)"
else
    echo "✗ QuizService database update failed"
fi

# SubscriptionService Database Update
echo -e "\n=== Updating SubscriptionService Database ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/SubscriptionService/SubscriptionService.API"
dotnet ef database update --project ../SubscriptionService.Infrastructure --context SubscriptionDbContext
if [ $? -eq 0 ]; then
    echo "✓ SubscriptionService database updated successfully (RankUp_SubscriptionDB)"
else
    echo "✗ SubscriptionService database update failed"
fi

# PaymentService Database Update
echo -e "\n=== Updating PaymentService Database ==="
cd "c:/Users/abhij/CascadeProjects/RankUpAPI/Services/PaymentService/PaymentService.API"
dotnet ef database update --project ../PaymentService.Infrastructure --context PaymentDbContext
if [ $? -eq 0 ]; then
    echo "✓ PaymentService database updated successfully (RankUp_PaymentDB)"
else
    echo "✗ PaymentService database update failed"
fi

# Return to root directory
cd "c:/Users/abhij/CascadeProjects/RankUpAPI"

echo -e "\n=== Database Updates Complete ==="
echo "All microservice databases have been created/updated."
echo "Each service now has its own dedicated database:"
echo "- RankUp_AdminDB"
echo "- RankUp_UserDB"
echo "- RankUp_ExamDB"
echo "- RankUp_QuestionDB"
echo "- RankUp_QuizDB"
echo "- RankUp_SubscriptionDB"
echo "- RankUp_PaymentDB"
