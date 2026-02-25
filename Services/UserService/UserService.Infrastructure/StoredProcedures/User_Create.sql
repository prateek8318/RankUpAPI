ALTER PROCEDURE [dbo].[User_Create]
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(20),
    @PasswordHash NVARCHAR(256),
    @ProfileImageUrl NVARCHAR(500),
    @EmailVerified BIT = 0,
    @IsActive BIT = 1,
    @PreferredLanguage NVARCHAR(10) = 'en',
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2,
    @UserId INT OUTPUT  -- ✅ Yeh add karo
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        INSERT INTO Users (
            Name, Email, PhoneNumber, PasswordHash, ProfileImageUrl,
            EmailVerified, IsActive, PreferredLanguage, CreatedAt, UpdatedAt
        )
        VALUES (
            @Name, @Email, @PhoneNumber, @PasswordHash, @ProfileImageUrl,
            @EmailVerified, @IsActive, @PreferredLanguage, @CreatedAt, @UpdatedAt
        );
        
        SET @UserId = SCOPE_IDENTITY();  -- ✅ OUTPUT param set karo
    END TRY
    BEGIN CATCH
        SET @UserId = 0;
    END CATCH
END