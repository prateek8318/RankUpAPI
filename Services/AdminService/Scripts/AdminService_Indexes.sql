USE [RankUp_AdminDB]
GO

PRINT 'Creating AdminService Database Indexes...';
PRINT '====================================================';

-- Admins table indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Admins_UserId' AND object_id = OBJECT_ID('Admins'))
BEGIN
    CREATE INDEX IX_Admins_UserId ON Admins(UserId);
    PRINT '✓ Created IX_Admins_UserId';
END
ELSE
    PRINT '✓ IX_Admins_UserId already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Admins_IsActive' AND object_id = OBJECT_ID('Admins'))
BEGIN
    CREATE INDEX IX_Admins_IsActive ON Admins(IsActive);
    PRINT '✓ Created IX_Admins_IsActive';
END
ELSE
    PRINT '✓ IX_Admins_IsActive already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Admins_CreatedAt' AND object_id = OBJECT_ID('Admins'))
BEGIN
    CREATE INDEX IX_Admins_CreatedAt ON Admins(CreatedAt);
    PRINT '✓ Created IX_Admins_CreatedAt';
END
ELSE
    PRINT '✓ IX_Admins_CreatedAt already exists';

-- Roles table indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Roles_IsActive' AND object_id = OBJECT_ID('Roles'))
BEGIN
    CREATE INDEX IX_Roles_IsActive ON Roles(IsActive);
    PRINT '✓ Created IX_Roles_IsActive';
END
ELSE
    PRINT '✓ IX_Roles_IsActive already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Roles_Name' AND object_id = OBJECT_ID('Roles'))
BEGIN
    CREATE INDEX IX_Roles_Name ON Roles(Name);
    PRINT '✓ Created IX_Roles_Name';
END
ELSE
    PRINT '✓ IX_Roles_Name already exists';

-- Permissions table indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Permissions_IsActive' AND object_id = OBJECT_ID('Permissions'))
BEGIN
    CREATE INDEX IX_Permissions_IsActive ON Permissions(IsActive);
    PRINT '✓ Created IX_Permissions_IsActive';
END
ELSE
    PRINT '✓ IX_Permissions_IsActive already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Permissions_Resource' AND object_id = OBJECT_ID('Permissions'))
BEGIN
    CREATE INDEX IX_Permissions_Resource ON Permissions(Resource);
    PRINT '✓ Created IX_Permissions_Resource';
END
ELSE
    PRINT '✓ IX_Permissions_Resource already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Permissions_Action' AND object_id = OBJECT_ID('Permissions'))
BEGIN
    CREATE INDEX IX_Permissions_Action ON Permissions(Action);
    PRINT '✓ Created IX_Permissions_Action';
END
ELSE
    PRINT '✓ IX_Permissions_Action already exists';

-- SubscriptionPlans table indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SubscriptionPlans_IsActive' AND object_id = OBJECT_ID('SubscriptionPlans'))
BEGIN
    CREATE INDEX IX_SubscriptionPlans_IsActive ON SubscriptionPlans(IsActive);
    PRINT '✓ Created IX_SubscriptionPlans_IsActive';
END
ELSE
    PRINT '✓ IX_SubscriptionPlans_IsActive already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SubscriptionPlans_ExamType' AND object_id = OBJECT_ID('SubscriptionPlans'))
BEGIN
    CREATE INDEX IX_SubscriptionPlans_ExamType ON SubscriptionPlans(ExamType);
    PRINT '✓ Created IX_SubscriptionPlans_ExamType';
END
ELSE
    PRINT '✓ IX_SubscriptionPlans_ExamType already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SubscriptionPlans_Price' AND object_id = OBJECT_ID('SubscriptionPlans'))
BEGIN
    CREATE INDEX IX_SubscriptionPlans_Price ON SubscriptionPlans(Price);
    PRINT '✓ Created IX_SubscriptionPlans_Price';
END
ELSE
    PRINT '✓ IX_SubscriptionPlans_Price already exists';

-- AuditLogs table indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_AdminId' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX IX_AuditLogs_AdminId ON AuditLogs(AdminId);
    PRINT '✓ Created IX_AuditLogs_AdminId';
END
ELSE
    PRINT '✓ IX_AuditLogs_AdminId already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_CreatedAt' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX IX_AuditLogs_CreatedAt ON AuditLogs(CreatedAt);
    PRINT '✓ Created IX_AuditLogs_CreatedAt';
END
ELSE
    PRINT '✓ IX_AuditLogs_CreatedAt already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_Action' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX IX_AuditLogs_Action ON AuditLogs(Action);
    PRINT '✓ Created IX_AuditLogs_Action';
END
ELSE
    PRINT '✓ IX_AuditLogs_Action already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_EntityType' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX IX_AuditLogs_EntityType ON AuditLogs(EntityType);
    PRINT '✓ Created IX_AuditLogs_EntityType';
END
ELSE
    PRINT '✓ IX_AuditLogs_EntityType already exists';

-- Composite indexes for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Admissions_IsActive_CreatedAt' AND object_id = OBJECT_ID('Admins'))
BEGIN
    CREATE INDEX IX_Admissions_IsActive_CreatedAt ON Admins(IsActive, CreatedAt DESC);
    PRINT '✓ Created IX_Admissions_IsActive_CreatedAt';
END
ELSE
    PRINT '✓ IX_Admissions_IsActive_CreatedAt already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_AdminId_CreatedAt' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX IX_AuditLogs_AdminId_CreatedAt ON AuditLogs(AdminId, CreatedAt DESC);
    PRINT '✓ Created IX_AuditLogs_AdminId_CreatedAt';
END
ELSE
    PRINT '✓ IX_AuditLogs_AdminId_CreatedAt already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SubscriptionPlans_IsActive_ExamType' AND object_id = OBJECT_ID('SubscriptionPlans'))
BEGIN
    CREATE INDEX IX_SubscriptionPlans_IsActive_ExamType ON SubscriptionPlans(IsActive, ExamType);
    PRINT '✓ Created IX_SubscriptionPlans_IsActive_ExamType';
END
ELSE
    PRINT '✓ IX_SubscriptionPlans_IsActive_ExamType already exists';

PRINT '====================================================';
PRINT 'ADMINSERVICE DATABASE INDEXES CREATED SUCCESSFULLY!';
PRINT '====================================================';
PRINT 'Indexes Created:';
PRINT '1. Admins table: UserId, IsActive, CreatedAt, IsActive+CreatedAt';
PRINT '2. Roles table: IsActive, Name';
PRINT '3. Permissions table: IsActive, Resource, Action';
PRINT '4. SubscriptionPlans table: IsActive, ExamType, Price, IsActive+ExamType';
PRINT '5. AuditLogs table: AdminId, CreatedAt, Action, EntityType, AdminId+CreatedAt';
PRINT '====================================================';
GO
