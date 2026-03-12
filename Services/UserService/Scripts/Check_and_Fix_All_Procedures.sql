-- Check all stored procedures with QUOTED_IDENTIFIER OFF
SELECT 
    name,
    type_desc,
    OBJECTPROPERTY(OBJECT_ID(name), 'ExecIsQuotedIdentOn') AS IsQuotedIdentifierOn,
    CASE 
        WHEN OBJECTPROPERTY(OBJECT_ID(name), 'ExecIsQuotedIdentOn') = 0 THEN 'QUOTED_IDENTIFIER OFF (NEEDS FIX)'
        ELSE 'QUOTED_IDENTIFIER ON (OK)'
    END AS Status
FROM sys.objects 
WHERE type = 'P' 
    AND OBJECTPROPERTY(OBJECT_ID(name), 'ExecIsQuotedIdentOn') = 0
ORDER BY name;
GO

-- Fix User_Create procedure as well since it might have the same issue
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('[dbo].[User_Create]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[User_Create];
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE [dbo].[User_Create]
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(20),
    @PasswordHash NVARCHAR(500),
    @ProfilePhoto NVARCHAR(500),
    @IsActive BIT,
    @PreferredLanguage NVARCHAR(10) = 'en',
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Users] (
        Name, Email, PhoneNumber, PasswordHash, ProfilePhoto, IsActive,
        PreferredLanguage, CreatedAt, UpdatedAt
    )
    VALUES (
        @Name, @Email, @PhoneNumber, @PasswordHash, @ProfilePhoto, @IsActive,
        @PreferredLanguage, @CreatedAt, @UpdatedAt
    );
    
    SET @UserId = SCOPE_IDENTITY();
    
    SELECT @UserId AS Id;
END
GO

-- Verify the procedures were created with correct QUOTED_IDENTIFIER setting
SELECT 
    name,
    type_desc,
    OBJECTPROPERTY(OBJECT_ID(name), 'ExecIsQuotedIdentOn') AS IsQuotedIdentifierOn
FROM sys.objects 
WHERE name IN ('User_Update', 'User_Create') AND type = 'P';
GO
