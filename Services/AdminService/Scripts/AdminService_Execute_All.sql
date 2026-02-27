

USE [RankUp_AdminDB]
GO

PRINT 'Starting AdminService Stored Procedures Execution...';
PRINT '====================================================';

-- Execute AdminService Procedures
PRINT 'Executing AdminService Procedures...';
:r "AdminService_StoredProcedures.sql"
GO

PRINT '====================================================';
PRINT 'ADMINSERVICE STORED PROCEDURES EXECUTED SUCCESSFULLY!';
PRINT '====================================================';
PRINT 'Services Completed:';
PRINT '1. Admin Procedures (6 procedures)';
PRINT '2. Role Procedures (5 procedures)';
PRINT '3. Permission Procedures (5 procedures)';
PRINT '4. SubscriptionPlan Procedures (5 procedures)';
PRINT '5. AuditLog Procedures (2 procedures)';
PRINT '====================================================';
PRINT 'Total Procedures: 23+';
PRINT 'Your AdminService Dapper repositories should now work correctly!';
PRINT '====================================================';
GO

-- Verification Script - Check critical procedures exist

PRINT 'Verifying critical AdminService stored procedures...';
PRINT '====================================================';

-- Check Admin procedures
SELECT 'AdminService' as ServiceName, 'Admin_GetAll' as ProcedureName, 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_GetAll]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END as Status
UNION ALL
SELECT 'AdminService', 'Admin_GetById', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_GetById]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'AdminService', 'Role_GetAll', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Role_GetAll]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'AdminService', 'Permission_GetAll', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permission_GetAll]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'AdminService', 'SubscriptionPlan_GetAll', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_GetAll]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'AdminService', 'AuditLog_Create', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLog_Create]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
ORDER BY ServiceName, ProcedureName;

PRINT '====================================================';
PRINT 'VERIFICATION COMPLETE!';
PRINT 'Check the results above - all should show EXISTS status';
PRINT 'If any show MISSING, re-run the AdminService_StoredProcedures.sql script';
PRINT '====================================================';
GO

-- =====================================================
-- Test Sample Procedures
-- =====================================================

PRINT 'Testing sample AdminService procedures...';
PRINT '====================================================';

-- Test Admin procedures
BEGIN TRY
    PRINT 'Testing AdminService.Admin_GetAll...';
    EXEC [dbo].[Admin_GetAll] @PageNumber = 1, @PageSize = 5;
    PRINT '✓ Admin_GetAll executed successfully';
END TRY
BEGIN CATCH
    PRINT '✗ Admin_GetAll failed: ' + ERROR_MESSAGE();
END CATCH

-- Test Role procedures
BEGIN TRY
    PRINT 'Testing AdminService.Role_GetAll...';
    EXEC [dbo].[Role_GetAll];
    PRINT '✓ Role_GetAll executed successfully';
END TRY
BEGIN CATCH
    PRINT '✗ Role_GetAll failed: ' + ERROR_MESSAGE();
END CATCH

-- Test Permission procedures
BEGIN TRY
    PRINT 'Testing AdminService.Permission_GetAll...';
    EXEC [dbo].[Permission_GetAll];
    PRINT '✓ Permission_GetAll executed successfully';
END TRY
BEGIN CATCH
    PRINT '✗ Permission_GetAll failed: ' + ERROR_MESSAGE();
END CATCH

-- Test SubscriptionPlan procedures
BEGIN TRY
    PRINT 'Testing AdminService.SubscriptionPlan_GetAll...';
    EXEC [dbo].[SubscriptionPlan_GetAll];
    PRINT '✓ SubscriptionPlan_GetAll executed successfully';
END TRY
BEGIN CATCH
    PRINT '✗ SubscriptionPlan_GetAll failed: ' + ERROR_MESSAGE();
END CATCH

PRINT '====================================================';
PRINT 'TESTING COMPLETE!';
PRINT 'If all tests passed, your AdminService Dapper repositories are ready!';
PRINT '====================================================';
GO
