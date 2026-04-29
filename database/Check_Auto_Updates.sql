-- Check if there are any stored procedures that might be updating subscription status
USE [RankUp_SubscriptionDB]
GO

PRINT '=== Checking for Auto-Update Stored Procedures ==='

-- Find stored procedures that might update subscription status
SELECT 
    ROUTINE_NAME,
    ROUTINE_DEFINITION
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_TYPE = 'PROCEDURE'
AND ROUTINE_SCHEMA = 'dbo'
AND (ROUTINE_DEFINITION LIKE '%UPDATE%' + CHAR(10) + '%UserSubscriptions%' 
     OR ROUTINE_DEFINITION LIKE '%Status%' + CHAR(10) + '%='
     OR ROUTINE_DEFINITION LIKE '%expired%'
     OR ROUTINE_DEFINITION LIKE '%Active%')
ORDER BY ROUTINE_NAME
GO

-- Check for any triggers on UserSubscriptions table
PRINT '=== Checking for Triggers on UserSubscriptions ==='
SELECT 
    trigger_name = name,
    trigger_owner = USER_NAME(OBJECTPROPERTY(OBJECT_ID, 'OwnerId')),
    object_schema = SCHEMA_NAME(OBJECTPROPERTY(OBJECT_ID, 'SchemaId')),
    object_name = OBJECT_NAME(OBJECTPROPERTY(OBJECT_ID, 'ParentId')),
    is_disabled = OBJECTPROPERTY(OBJECT_ID, 'ExecIsDisabled'),
    is_instead_of_trigger = OBJECTPROPERTY(OBJECT_ID, 'IsInsteadOfTrigger')
FROM sys.objects 
WHERE type = 'TR' 
AND parent_object_id = OBJECT_ID('UserSubscriptions')
GO

-- Check recent activity on UserSubscriptions
PRINT '=== Recent UserSubscriptions Activity ==='
SELECT TOP 10
    us.Id,
    us.UserId,
    us.Status,
    us.ValidTill,
    us.UpdatedAt,
    CASE 
        WHEN us.UpdatedAt > DATEADD(MINUTE, -5, GETDATE()) THEN 'Updated in last 5 minutes'
        WHEN us.UpdatedAt > DATEADD(HOUR, -1, GETDATE()) THEN 'Updated in last hour'
        ELSE 'Older'
    END AS UpdateActivity
FROM UserSubscriptions us
WHERE us.UserId = 42
ORDER BY us.UpdatedAt DESC
GO

PRINT '=== Check Complete ==='
