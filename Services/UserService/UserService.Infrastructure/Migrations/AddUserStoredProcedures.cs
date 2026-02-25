using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Infrastructure.Migrations
{
    public partial class AddUserStoredProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // User_GetAll with pagination
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_GetAll]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[User_GetAll]
                        @Page INT = 1,
                        @PageSize INT = 50
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        DECLARE @Offset INT = (@Page - 1) * @PageSize;
                        
                        SELECT 
                            Id, Name, Email, PhoneNumber, CountryCode, Gender, DateOfBirth, Qualification,
                            PreferredLanguage, ProfilePhoto, PreferredExam, StateId, LanguageId,
                            QualificationId, ExamId, CategoryId, StreamId, RefreshToken,
                            RefreshTokenExpiryTime, IsPhoneVerified, InterestedInIntlExam,
                            IsActive, CreatedAt, UpdatedAt, LastLoginAt, GoogleId
                        FROM [Users]
                        ORDER BY CreatedAt DESC
                        OFFSET @Offset ROWS
                        FETCH NEXT @PageSize ROWS ONLY;
                    END;');
                END");

            // User_GetTotalCount
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_GetTotalCount]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[User_GetTotalCount]
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        SELECT COUNT(*) 
                        FROM [Users];
                    END;');
                END");

            // User_GetDailyActiveCount
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_GetDailyActiveCount]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[User_GetDailyActiveCount]
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        SELECT COUNT(*) 
                        FROM [Users]
                        WHERE LastLoginAt >= DATEADD(DAY, -1, GETDATE())
                        AND IsActive = 1;
                    END;');
                END");

            // User_GetById
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_GetById]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[User_GetById]
                        @UserId INT
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        SELECT 
                            Id, Name, Email, PhoneNumber, CountryCode, Gender, DateOfBirth, Qualification,
                            PreferredLanguage, ProfilePhoto, PreferredExam, StateId, LanguageId,
                            QualificationId, ExamId, CategoryId, StreamId, RefreshToken,
                            RefreshTokenExpiryTime, IsPhoneVerified, InterestedInIntlExam,
                            IsActive, CreatedAt, UpdatedAt, LastLoginAt, GoogleId
                        FROM [Users]
                        WHERE Id = @UserId AND IsActive = 1;
                    END;');
                END");

            // User_GetByPhoneNumber
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_GetByPhoneNumber]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[User_GetByPhoneNumber]
                        @PhoneNumber NVARCHAR(20)
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        SELECT 
                            Id,
                            Name,
                            Email,
                            PhoneNumber,
                            PasswordHash,
                            CountryCode,
                            Gender,
                            DateOfBirth,
                            Qualification,
                            PreferredLanguage,
                            ProfilePhoto,
                            PreferredExam,
                            StateId,
                            LanguageId,
                            QualificationId,
                            ExamId,
                            CategoryId,
                            StreamId,
                            RefreshToken,
                            RefreshTokenExpiryTime,
                            IsPhoneVerified,
                            InterestedInIntlExam,
                            IsActive,
                            CreatedAt,
                            UpdatedAt,
                            LastLoginAt,
                            GoogleId
                        FROM [Users]
                        WHERE PhoneNumber = @PhoneNumber AND IsActive = 1;
                    END;');
                END");

            // User_Create
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_Create]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[User_Create]
                        @Name NVARCHAR(100),
                        @Email NVARCHAR(100),
                        @PhoneNumber NVARCHAR(20),
                        @PasswordHash NVARCHAR(256),
                        @ProfileImageUrl NVARCHAR(500),
                        @EmailVerified BIT = 0,
                        @IsActive BIT = 1,
                        @PreferredLanguage NVARCHAR(10) = ''en'',
                        @CreatedAt DATETIME2,
                        @UpdatedAt DATETIME2,
                        @UserId INT OUTPUT
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        INSERT INTO Users (
                            Name, Email, PhoneNumber, PasswordHash, ProfileImageUrl,
                            EmailVerified, IsActive, PreferredLanguage, CreatedAt, UpdatedAt
                        )
                        VALUES (
                            @Name, @Email, @PhoneNumber, @PasswordHash, @ProfileImageUrl,
                            @EmailVerified, @IsActive, @PreferredLanguage, @CreatedAt, @UpdatedAt
                        );
                        
                        SET @UserId = SCOPE_IDENTITY();
                    END;');
                END");

            // User_Update
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_Update]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[User_Update]
                        @UserId INT,
                        @Name NVARCHAR(100) = NULL,
                        @Email NVARCHAR(100) = NULL,
                        @PhoneNumber NVARCHAR(20) = NULL,
                        @ProfileImageUrl NVARCHAR(500) = NULL,
                        @EmailVerified BIT = NULL,
                        @IsActive BIT = NULL,
                        @PreferredLanguage NVARCHAR(10) = NULL,
                        @UpdatedAt DATETIME2
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        UPDATE Users
                        SET 
                            Name = ISNULL(@Name, Name),
                            Email = ISNULL(@Email, Email),
                            PhoneNumber = ISNULL(@PhoneNumber, PhoneNumber),
                            ProfileImageUrl = ISNULL(@ProfileImageUrl, ProfileImageUrl),
                            EmailVerified = ISNULL(@EmailVerified, EmailVerified),
                            IsActive = ISNULL(@IsActive, IsActive),
                            PreferredLanguage = ISNULL(@PreferredLanguage, PreferredLanguage),
                            UpdatedAt = @UpdatedAt
                        WHERE Id = @UserId;
                    END;');
                END");

            // User_Delete (already exists, but adding for completeness)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_Delete]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[User_Delete]
                        @UserId INT
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        BEGIN TRANSACTION;
                        
                        -- Delete user's test attempts
                        DELETE FROM UserTestAttempts WHERE UserId = @UserId;
                        
                        -- Delete user's quiz attempts
                        DELETE FROM QuizAttempts WHERE UserId = @UserId;
                        
                        -- Delete user's subscriptions
                        DELETE FROM UserSubscriptions WHERE UserId = @UserId;
                        
                        -- Delete user's profile images
                        DELETE FROM UserProfiles WHERE UserId = @UserId;
                        
                        -- Delete user's preferences
                        DELETE FROM UserPreferences WHERE UserId = @UserId;
                        
                        -- Delete user's social login accounts
                        DELETE FROM UserSocialLogins WHERE UserId = @UserId;
                        
                        -- Delete user's refresh tokens
                        DELETE FROM RefreshTokens WHERE UserId = @UserId;
                        
                        -- Finally delete user
                        DELETE FROM Users WHERE Id = @UserId;
                        
                        COMMIT TRANSACTION;
                        
                        SELECT 1 AS Result, ''User deleted successfully'' AS Message;
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0
                            ROLLBACK TRANSACTION;
                            
                        SELECT 0 AS Result, 
                               ERROR_MESSAGE() AS Message,
                               ERROR_NUMBER() AS ErrorNumber;
                    END CATCH
                    END;');
                END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[User_GetAll]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[User_GetTotalCount]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[User_GetDailyActiveCount]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[User_GetById]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[User_GetByPhoneNumber]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[User_Create]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[User_Update]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[User_Delete]");
        }
    }
}
