-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to update user
-- =============================================
CREATE PROCEDURE [dbo].[User_Update]
    @UserId INT,
    @Name NVARCHAR(100) = NULL,
    @Email NVARCHAR(100) = NULL,
    @PhoneNumber NVARCHAR(20) = NULL,
    @ProfileImageUrl NVARCHAR(500) = NULL,
    @EmailVerified BIT = NULL,
    @IsActive BIT = NULL,
    @PreferredLanguage NVARCHAR(10) = NULL,
    @LastLoginAt DATETIME2 = NULL,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        UPDATE Users
        SET 
            Name = ISNULL(@Name, Name),
            Email = ISNULL(@Email, Email),
            PhoneNumber = ISNULL(@PhoneNumber, PhoneNumber),
            ProfileImageUrl = ISNULL(@ProfileImageUrl, ProfileImageUrl),
            EmailVerified = ISNULL(@EmailVerified, EmailVerified),
            IsActive = ISNULL(@IsActive, IsActive),
            PreferredLanguage = ISNULL(@PreferredLanguage, PreferredLanguage),
            LastLoginAt = ISNULL(@LastLoginAt, LastLoginAt),
            UpdatedAt = @UpdatedAt
        WHERE Id = @UserId;
        
        SELECT @@ROWCOUNT AS RowsAffected, 'User updated successfully' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS RowsAffected, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
