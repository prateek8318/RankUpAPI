@echo off
echo ========================================
echo Running Subscription Service Stored Procedures
echo ========================================

echo.
echo Checking SQL Server connection...
sqlcmd -S localhost -E -Q "SELECT 1" >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Cannot connect to SQL Server. Please check:
    echo 1. SQL Server is running
    echo 2. Windows Authentication is enabled
    echo 3. You have proper permissions
    pause
    exit /b 1
)

echo.
echo Connection successful! Creating stored procedures...
echo.

echo Creating SubscriptionPlan procedures...
sqlcmd -S localhost -E -i SubscriptionService_StoredProcedures.sql

if %errorlevel% equ 0 (
    echo.
    echo ========================================
    echo SUCCESS: All stored procedures created!
    echo ========================================
    echo.
    echo Stored procedures created:
    echo - SubscriptionPlan_GetAll
    echo - SubscriptionPlan_GetById  
    echo - SubscriptionPlan_Create
    echo - SubscriptionPlan_Update
    echo - SubscriptionPlan_Delete
    echo - SubscriptionPlan_GetByExamCategory
    echo - SubscriptionPlan_GetByExamId
    echo - SubscriptionPlan_GetActive
    echo - SubscriptionPlan_GetByPlanType
    echo - SubscriptionPlan_ExistsByName
    echo.
    echo - UserSubscription_GetAll
    echo - UserSubscription_GetById
    echo - UserSubscription_GetByUserId
    echo - UserSubscription_GetActiveByUserId
    echo - UserSubscription_Create
    echo - UserSubscription_Update
    echo - UserSubscription_Delete
    echo - UserSubscription_Cancel
    echo - UserSubscription_GetActive
    echo - UserSubscription_GetExpiring
    echo - UserSubscription_Renew
    echo - UserSubscription_GetByUserIdWithHistory
    echo - UserSubscription_GetByRazorpayOrderId
    echo - UserSubscription_GetByRazorpayPaymentId
    echo.
    echo Subscription CRUD operations are now ready!
) else (
    echo.
    echo ========================================
    echo ERROR: Failed to create stored procedures
    echo ========================================
    echo Please check the error messages above
)

echo.
pause
