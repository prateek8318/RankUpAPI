USE [master]
GO

-- Drop if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RankUp_AdminDB')
BEGIN
    ALTER DATABASE [RankUp_AdminDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [RankUp_AdminDB];
END
GO

-- Create with Linux path
CREATE DATABASE [RankUp_AdminDB]
ON PRIMARY 
( NAME = N'RankUp_AdminDB', FILENAME = N'/var/opt/mssql/data/RankUp_AdminDB.mdf', SIZE = 8192KB, FILEGROWTH = 65536KB )
LOG ON 
( NAME = N'RankUp_AdminDB_log', FILENAME = N'/var/opt/mssql/data/RankUp_AdminDB_log.ldf', SIZE = 8192KB, FILEGROWTH = 65536KB )
GO

USE [RankUp_AdminDB]
GO

-- Migrations History
CREATE TABLE [dbo].[__EFMigrationsHistory](
    [MigrationId] [nvarchar](150) NOT NULL,
    [ProductVersion] [nvarchar](32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
)
GO

-- AdminActivityLogs Table
CREATE TABLE [dbo].[AdminActivityLogs](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [AdminId] [int] NOT NULL,
    [Action] [nvarchar](100) NOT NULL,
    [Resource] [nvarchar](200) NULL,
    [ResourceId] [int] NULL,
    [Details] [nvarchar](1000) NULL,
    [IpAddress] [nvarchar](50) NULL,
    [UserAgent] [nvarchar](500) NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_AdminActivityLogs] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- AdminRoles Table
CREATE TABLE [dbo].[AdminRoles](
    [AdminId] [int] NOT NULL,
    [RoleId] [int] NOT NULL,
    [Id] [int] NOT NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_AdminRoles] PRIMARY KEY CLUSTERED ([AdminId] ASC, [RoleId] ASC)
)
GO

-- Admins Table
CREATE TABLE [dbo].[Admins](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [int] NOT NULL,
    [Role] [nvarchar](50) NOT NULL,
    [IsTwoFactorEnabled] [bit] NOT NULL DEFAULT 0,
    [TwoFactorSecret] [nvarchar](max) NULL,
    [RefreshToken] [nvarchar](max) NULL,
    [RefreshTokenExpiryTime] [datetime2](7) NULL,
    [LastLoginAt] [datetime2](7) NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Admins] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- AdminSessions Table
CREATE TABLE [dbo].[AdminSessions](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [AdminId] [int] NOT NULL,
    [Token] [nvarchar](max) NOT NULL,
    [RefreshToken] [nvarchar](max) NOT NULL,
    [ExpiresAt] [datetime2](7) NOT NULL,
    [DeviceInfo] [nvarchar](max) NULL,
    [IpAddress] [nvarchar](max) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    CONSTRAINT [PK_AdminSessions] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- AuditLogs Table
CREATE TABLE [dbo].[AuditLogs](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [AdminId] [int] NOT NULL,
    [Action] [nvarchar](100) NOT NULL,
    [ServiceName] [nvarchar](100) NOT NULL,
    [Endpoint] [nvarchar](200) NOT NULL,
    [HttpMethod] [nvarchar](50) NOT NULL,
    [RequestPayload] [nvarchar](2000) NULL,
    [ResponsePayload] [nvarchar](2000) NULL,
    [StatusCode] [int] NULL,
    [IpAddress] [nvarchar](50) NULL,
    [UserAgent] [nvarchar](500) NULL,
    [ResponseTimeMs] [bigint] NULL,
    [ErrorMessage] [nvarchar](1000) NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- DashboardCaches Table
CREATE TABLE [dbo].[DashboardCaches](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [CacheKey] [nvarchar](100) NOT NULL,
    [CacheData] [nvarchar](max) NOT NULL,
    [ExpiresAt] [datetime2](7) NULL,
    [CacheType] [nvarchar](50) NOT NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_DashboardCaches] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- ExportLogs Table
CREATE TABLE [dbo].[ExportLogs](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [AdminId] [int] NOT NULL,
    [ExportType] [nvarchar](100) NOT NULL,
    [FilePath] [nvarchar](500) NULL,
    [FileName] [nvarchar](200) NULL,
    [FileSizeBytes] [bigint] NULL,
    [Format] [nvarchar](50) NOT NULL,
    [RecordCount] [int] NULL,
    [Status] [int] NOT NULL,
    [ErrorMessage] [nvarchar](1000) NULL,
    [CompletedAt] [datetime2](7) NULL,
    [FilterCriteria] [nvarchar](2000) NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_ExportLogs] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- Permissions Table
CREATE TABLE [dbo].[Permissions](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    [Resource] [nvarchar](200) NULL,
    [Action] [nvarchar](50) NULL,
    [Description] [nvarchar](500) NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- RolePermissions Table
CREATE TABLE [dbo].[RolePermissions](
    [RoleId] [int] NOT NULL,
    [PermissionId] [int] NOT NULL,
    [Id] [int] NOT NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY CLUSTERED ([RoleId] ASC, [PermissionId] ASC)
)
GO

-- Roles Table
CREATE TABLE [dbo].[Roles](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    [Description] [nvarchar](500) NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- Indexes
CREATE NONCLUSTERED INDEX [IX_AdminActivityLogs_AdminId_CreatedAt] ON [dbo].[AdminActivityLogs]([AdminId] ASC, [CreatedAt] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_AdminRoles_RoleId] ON [dbo].[AdminRoles]([RoleId] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_Admins_UserId] ON [dbo].[Admins]([UserId] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_AdminSessions_AdminId] ON [dbo].[AdminSessions]([AdminId] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_AuditLogs_AdminId_CreatedAt] ON [dbo].[AuditLogs]([AdminId] ASC, [CreatedAt] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_AuditLogs_ServiceName] ON [dbo].[AuditLogs]([ServiceName] ASC)
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_DashboardCaches_CacheKey] ON [dbo].[DashboardCaches]([CacheKey] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_DashboardCaches_CacheType] ON [dbo].[DashboardCaches]([CacheType] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_DashboardCaches_ExpiresAt] ON [dbo].[DashboardCaches]([ExpiresAt] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_ExportLogs_AdminId] ON [dbo].[ExportLogs]([AdminId] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_ExportLogs_ExportType] ON [dbo].[ExportLogs]([ExportType] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_ExportLogs_Status] ON [dbo].[ExportLogs]([Status] ASC)
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Permissions_Name_Resource_Action] ON [dbo].[Permissions]([Name] ASC, [Resource] ASC, [Action] ASC) WHERE ([Resource] IS NOT NULL AND [Action] IS NOT NULL)
GO
CREATE NONCLUSTERED INDEX [IX_RolePermissions_PermissionId] ON [dbo].[RolePermissions]([PermissionId] ASC)
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Roles_Name] ON [dbo].[Roles]([Name] ASC)
GO

-- Foreign Keys
ALTER TABLE [dbo].[AdminActivityLogs] ADD CONSTRAINT [FK_AdminActivityLogs_Admins_AdminId] FOREIGN KEY([AdminId]) REFERENCES [dbo].[Admins]([Id]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdminRoles] ADD CONSTRAINT [FK_AdminRoles_Admins_AdminId] FOREIGN KEY([AdminId]) REFERENCES [dbo].[Admins]([Id]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdminRoles] ADD CONSTRAINT [FK_AdminRoles_Roles_RoleId] FOREIGN KEY([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdminSessions] ADD CONSTRAINT [FK_AdminSessions_Admins_AdminId] FOREIGN KEY([AdminId]) REFERENCES [dbo].[Admins]([Id]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AuditLogs] ADD CONSTRAINT [FK_AuditLogs_Admins_AdminId] FOREIGN KEY([AdminId]) REFERENCES [dbo].[Admins]([Id])
GO
ALTER TABLE [dbo].[ExportLogs] ADD CONSTRAINT [FK_ExportLogs_Admins_AdminId] FOREIGN KEY([AdminId]) REFERENCES [dbo].[Admins]([Id])
GO
ALTER TABLE [dbo].[RolePermissions] ADD CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY([PermissionId]) REFERENCES [dbo].[Permissions]([Id]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RolePermissions] ADD CONSTRAINT [FK_RolePermissions_Roles_RoleId] FOREIGN KEY([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE
GO

-- Seed Data
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260114083816_InitialCreate', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260115130149_AddAuditLogExportLogDashboardCache', N'8.0.0')
GO

SET IDENTITY_INSERT [dbo].[Admins] ON
INSERT [dbo].[Admins] ([Id], [UserId], [Role], [IsTwoFactorEnabled], [TwoFactorSecret], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [CreatedAt], [UpdatedAt], [IsActive])
VALUES (1, 1, N'Admin', 0, NULL, NULL, NULL, NULL, CAST(N'2026-03-10T08:50:19.3766667' AS DateTime2), CAST(N'2026-03-10T08:50:19.3766667' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[Admins] OFF
GO

-- Stored Procedures
CREATE PROCEDURE [dbo].[SubjectLanguage_Create]
    @SubjectId INT, @LanguageId INT, @Name NVARCHAR(200),
    @IsActive BIT = 1, @CreatedAt DATETIME2, @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO SubjectLanguages (SubjectId, LanguageId, Name, IsActive, CreatedAt, UpdatedAt)
        OUTPUT INSERTED.Id
        VALUES (@SubjectId, @LanguageId, @Name, @IsActive, @CreatedAt, @UpdatedAt);
        RETURN 0;
    END TRY
    BEGIN CATCH
        RETURN ERROR_NUMBER();
    END CATCH
END
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_Delete] @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE SubjectLanguages SET IsActive=0, UpdatedAt=GETUTCDATE() WHERE Id=@Id;
        RETURN 0;
    END TRY
    BEGIN CATCH
        RETURN ERROR_NUMBER();
    END CATCH
END
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_GetById] @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT sl.Id, sl.SubjectId, sl.LanguageId, sl.Name, sl.IsActive, sl.CreatedAt, sl.UpdatedAt,
           l.Id as LanguageId, l.Code as LanguageCode, l.Name as LanguageName
    FROM SubjectLanguages sl WITH (NOLOCK)
    INNER JOIN Languages l WITH (NOLOCK) ON sl.LanguageId = l.Id
    WHERE sl.Id=@Id AND sl.IsActive=1;
END
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_GetBySubjectId] @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT sl.Id, sl.SubjectId, sl.LanguageId, sl.Name, sl.IsActive, sl.CreatedAt, sl.UpdatedAt,
           l.Id as LanguageId, l.Code as LanguageCode, l.Name as LanguageName
    FROM SubjectLanguages sl WITH (NOLOCK)
    INNER JOIN Languages l WITH (NOLOCK) ON sl.LanguageId = l.Id
    WHERE sl.SubjectId=@SubjectId AND sl.IsActive=1
    ORDER BY l.Name;
END
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_Update]
    @Id INT, @SubjectId INT, @LanguageId INT, @Name NVARCHAR(200),
    @IsActive BIT = 1, @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE SubjectLanguages
        SET SubjectId=@SubjectId, LanguageId=@LanguageId, Name=@Name,
            IsActive=@IsActive, UpdatedAt=@UpdatedAt
        WHERE Id=@Id;
        RETURN 0;
    END TRY
    BEGIN CATCH
        RETURN ERROR_NUMBER();
    END CATCH
END
GO