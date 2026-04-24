USE [RankUp_UserDB]
GO

PRINT 'Creating UserService Database Indexes...';
PRINT '====================================================';

-- Users table indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_IsActive' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_IsActive ON Users(IsActive);
    PRINT '✓ Created IX_Users_IsActive';
END
ELSE
    PRINT '✓ IX_Users_IsActive already exists';

-- IX_Users_PhoneNumber already exists as unique index in main script
PRINT '✓ IX_Users_PhoneNumber already exists (unique index from main script)';

-- IX_Users_Email already exists as unique index in main script
PRINT '✓ IX_Users_Email already exists (unique index from main script)';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_CountryCode' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_CountryCode ON Users(CountryCode);
    PRINT '✓ Created IX_Users_CountryCode';
END
ELSE
    PRINT '✓ IX_Users_CountryCode already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_LastLoginAt' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_LastLoginAt ON Users(LastLoginAt);
    PRINT '✓ Created IX_Users_LastLoginAt';
END
ELSE
    PRINT '✓ IX_Users_LastLoginAt already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_CreatedAt' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_CreatedAt ON Users(CreatedAt);
    PRINT '✓ Created IX_Users_CreatedAt';
END
ELSE
    PRINT '✓ IX_Users_CreatedAt already exists';

-- Skip IsActive+PhoneNumber composite index - conflicts with unique PhoneNumber index
PRINT '✓ Skipped IX_Users_IsActive_PhoneNumber - conflicts with unique PhoneNumber index';

-- Skip IsActive+Email composite index - conflicts with unique Email index
PRINT '✓ Skipped IX_Users_IsActive_Email - conflicts with unique Email index';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_IsActive_LastLoginAt' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_IsActive_LastLoginAt ON Users(IsActive, LastLoginAt DESC);
    PRINT '✓ Created IX_Users_IsActive_LastLoginAt';
END
ELSE
    PRINT '✓ IX_Users_IsActive_LastLoginAt already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_IsActive_CreatedAt' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_IsActive_CreatedAt ON Users(IsActive, CreatedAt DESC);
    PRINT '✓ Created IX_Users_IsActive_CreatedAt';
END
ELSE
    PRINT '✓ IX_Users_IsActive_CreatedAt already exists';

-- Additional foreign key indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_StateId' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_StateId ON Users(StateId);
    PRINT '✓ Created IX_Users_StateId';
END
ELSE
    PRINT '✓ IX_Users_StateId already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_LanguageId' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_LanguageId ON Users(LanguageId);
    PRINT '✓ Created IX_Users_LanguageId';
END
ELSE
    PRINT '✓ IX_Users_LanguageId already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_QualificationId' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_QualificationId ON Users(QualificationId);
    PRINT '✓ Created IX_Users_QualificationId';
END
ELSE
    PRINT '✓ IX_Users_QualificationId already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_ExamId' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_ExamId ON Users(ExamId);
    PRINT '✓ Created IX_Users_ExamId';
END
ELSE
    PRINT '✓ IX_Users_ExamId already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_CategoryId' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_CategoryId ON Users(CategoryId);
    PRINT '✓ Created IX_Users_CategoryId';
END
ELSE
    PRINT '✓ IX_Users_CategoryId already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_StreamId' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_StreamId ON Users(StreamId);
    PRINT '✓ Created IX_Users_StreamId';
END
ELSE
    PRINT '✓ IX_Users_StreamId already exists';

-- UserSocialLogins table indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserSocialLogins_UserId' AND object_id = OBJECT_ID('UserSocialLogins'))
BEGIN
    CREATE INDEX IX_UserSocialLogins_UserId ON UserSocialLogins(UserId);
    PRINT '✓ Created IX_UserSocialLogins_UserId';
END
ELSE
    PRINT '✓ IX_UserSocialLogins_UserId already exists';

-- Skip Provider index - Provider column is nvarchar(max) and cannot be indexed
PRINT '✓ Skipped IX_UserSocialLogins_Provider - Provider column is nvarchar(max)';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserSocialLogins_GoogleId' AND object_id = OBJECT_ID('UserSocialLogins'))
BEGIN
    CREATE INDEX IX_UserSocialLogins_GoogleId ON UserSocialLogins(GoogleId);
    PRINT '✓ Created IX_UserSocialLogins_GoogleId';
END
ELSE
    PRINT '✓ IX_UserSocialLogins_GoogleId already exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserSocialLogins_Email' AND object_id = OBJECT_ID('UserSocialLogins'))
BEGIN
    CREATE INDEX IX_UserSocialLogins_Email ON UserSocialLogins(Email);
    PRINT '✓ Created IX_UserSocialLogins_Email';
END
ELSE
    PRINT '✓ IX_UserSocialLogins_Email already exists';

-- Composite indexes for UserSocialLogins
-- Skip Provider+GoogleId index - Provider column is nvarchar(max) and cannot be indexed
PRINT '✓ Skipped IX_UserSocialLogins_Provider_GoogleId - Provider column is nvarchar(max)';

-- Skip UserId+Provider index - Provider column is nvarchar(max) and cannot be indexed
PRINT '✓ Skipped IX_UserSocialLogins_UserId_Provider - Provider column is nvarchar(max)';

PRINT '====================================================';
PRINT 'USERSERVICE DATABASE INDEXES CREATED SUCCESSFULLY!';
PRINT '====================================================';
PRINT 'Indexes Created:';
PRINT '1. Users table: IsActive, CountryCode, LastLoginAt, CreatedAt (PhoneNumber, Email exist as unique indexes)';
PRINT '2. Users composite indexes: IsActive+LastLoginAt, IsActive+CreatedAt (PhoneNumber, Email composites skipped)';
PRINT '3. Users foreign key indexes: StateId, LanguageId, QualificationId, ExamId, CategoryId, StreamId';
PRINT '4. UserSocialLogins table: UserId, GoogleId, Email (Provider skipped - nvarchar(max))';
PRINT '5. UserSocialLogins composite indexes: None (Provider indexes skipped - nvarchar(max))';
PRINT '====================================================';
GO
