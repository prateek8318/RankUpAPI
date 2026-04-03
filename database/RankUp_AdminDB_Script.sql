USE [RankUp_AdminDB]
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_Update]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP PROCEDURE [dbo].[SubjectLanguage_Update]
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_GetBySubjectId]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP PROCEDURE [dbo].[SubjectLanguage_GetBySubjectId]
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_GetById]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP PROCEDURE [dbo].[SubjectLanguage_GetById]
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_Delete]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP PROCEDURE [dbo].[SubjectLanguage_Delete]
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_Create]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP PROCEDURE [dbo].[SubjectLanguage_Create]
GO
ALTER TABLE [dbo].[RolePermissions] DROP CONSTRAINT [FK_RolePermissions_Roles_RoleId]
GO
ALTER TABLE [dbo].[RolePermissions] DROP CONSTRAINT [FK_RolePermissions_Permissions_PermissionId]
GO
ALTER TABLE [dbo].[ExportLogs] DROP CONSTRAINT [FK_ExportLogs_Admins_AdminId]
GO
ALTER TABLE [dbo].[AuditLogs] DROP CONSTRAINT [FK_AuditLogs_Admins_AdminId]
GO
ALTER TABLE [dbo].[AdminSessions] DROP CONSTRAINT [FK_AdminSessions_Admins_AdminId]
GO
ALTER TABLE [dbo].[AdminRoles] DROP CONSTRAINT [FK_AdminRoles_Roles_RoleId]
GO
ALTER TABLE [dbo].[AdminRoles] DROP CONSTRAINT [FK_AdminRoles_Admins_AdminId]
GO
ALTER TABLE [dbo].[AdminActivityLogs] DROP CONSTRAINT [FK_AdminActivityLogs_Admins_AdminId]
GO
ALTER TABLE [dbo].[Roles] DROP CONSTRAINT [DF__Roles__IsActive__5441852A]
GO
ALTER TABLE [dbo].[Roles] DROP CONSTRAINT [DF__Roles__CreatedAt__534D60F1]
GO
ALTER TABLE [dbo].[RolePermissions] DROP CONSTRAINT [DF__RolePermi__IsAct__66603565]
GO
ALTER TABLE [dbo].[RolePermissions] DROP CONSTRAINT [DF__RolePermi__Creat__656C112C]
GO
ALTER TABLE [dbo].[Permissions] DROP CONSTRAINT [DF__Permissio__IsAct__5070F446]
GO
ALTER TABLE [dbo].[Permissions] DROP CONSTRAINT [DF__Permissio__Creat__4F7CD00D]
GO
ALTER TABLE [dbo].[ExportLogs] DROP CONSTRAINT [DF__ExportLog__Creat__71D1E811]
GO
ALTER TABLE [dbo].[DashboardCaches] DROP CONSTRAINT [DF__Dashboard__Creat__6EF57B66]
GO
ALTER TABLE [dbo].[AuditLogs] DROP CONSTRAINT [DF__AuditLogs__Creat__6B24EA82]
GO
ALTER TABLE [dbo].[AdminSessions] DROP CONSTRAINT [DF__AdminSess__Creat__5BE2A6F2]
GO
ALTER TABLE [dbo].[AdminSessions] DROP CONSTRAINT [DF__AdminSess__IsAct__5AEE82B9]
GO
ALTER TABLE [dbo].[Admins] DROP CONSTRAINT [DF__Admins__IsActive__4CA06362]
GO
ALTER TABLE [dbo].[Admins] DROP CONSTRAINT [DF__Admins__CreatedA__4BAC3F29]
GO
ALTER TABLE [dbo].[AdminRoles] DROP CONSTRAINT [DF__AdminRole__IsAct__60A75C0F]
GO
ALTER TABLE [dbo].[AdminRoles] DROP CONSTRAINT [DF__AdminRole__Creat__5FB337D6]
GO
ALTER TABLE [dbo].[AdminActivityLogs] DROP CONSTRAINT [DF__AdminActi__Creat__571DF1D5]
GO
/****** Object:  Index [IX_Roles_Name]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_Roles_Name] ON [dbo].[Roles]
GO
/****** Object:  Index [IX_RolePermissions_PermissionId]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_RolePermissions_PermissionId] ON [dbo].[RolePermissions]
GO
/****** Object:  Index [IX_Permissions_Name_Resource_Action]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_Permissions_Name_Resource_Action] ON [dbo].[Permissions]
GO
/****** Object:  Index [IX_ExportLogs_Status]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_ExportLogs_Status] ON [dbo].[ExportLogs]
GO
/****** Object:  Index [IX_ExportLogs_ExportType]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_ExportLogs_ExportType] ON [dbo].[ExportLogs]
GO
/****** Object:  Index [IX_ExportLogs_AdminId]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_ExportLogs_AdminId] ON [dbo].[ExportLogs]
GO
/****** Object:  Index [IX_DashboardCaches_ExpiresAt]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_DashboardCaches_ExpiresAt] ON [dbo].[DashboardCaches]
GO
/****** Object:  Index [IX_DashboardCaches_CacheType]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_DashboardCaches_CacheType] ON [dbo].[DashboardCaches]
GO
/****** Object:  Index [IX_DashboardCaches_CacheKey]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_DashboardCaches_CacheKey] ON [dbo].[DashboardCaches]
GO
/****** Object:  Index [IX_AuditLogs_ServiceName]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_AuditLogs_ServiceName] ON [dbo].[AuditLogs]
GO
/****** Object:  Index [IX_AuditLogs_AdminId_CreatedAt]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_AuditLogs_AdminId_CreatedAt] ON [dbo].[AuditLogs]
GO
/****** Object:  Index [IX_AdminSessions_AdminId]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_AdminSessions_AdminId] ON [dbo].[AdminSessions]
GO
/****** Object:  Index [IX_Admins_UserId]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_Admins_UserId] ON [dbo].[Admins]
GO
/****** Object:  Index [IX_AdminRoles_RoleId]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_AdminRoles_RoleId] ON [dbo].[AdminRoles]
GO
/****** Object:  Index [IX_AdminActivityLogs_AdminId_CreatedAt]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP INDEX [IX_AdminActivityLogs_AdminId_CreatedAt] ON [dbo].[AdminActivityLogs]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 4/3/2026 11:59:58 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
DROP TABLE [dbo].[Roles]
GO
/****** Object:  Table [dbo].[RolePermissions]    Script Date: 4/3/2026 11:59:58 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RolePermissions]') AND type in (N'U'))
DROP TABLE [dbo].[RolePermissions]
GO
/****** Object:  Table [dbo].[Permissions]    Script Date: 4/3/2026 11:59:58 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permissions]') AND type in (N'U'))
DROP TABLE [dbo].[Permissions]
GO
/****** Object:  Table [dbo].[ExportLogs]    Script Date: 4/3/2026 11:59:58 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExportLogs]') AND type in (N'U'))
DROP TABLE [dbo].[ExportLogs]
GO
/****** Object:  Table [dbo].[DashboardCaches]    Script Date: 4/3/2026 11:59:58 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DashboardCaches]') AND type in (N'U'))
DROP TABLE [dbo].[DashboardCaches]
GO
/****** Object:  Table [dbo].[AuditLogs]    Script Date: 4/3/2026 11:59:58 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND type in (N'U'))
DROP TABLE [dbo].[AuditLogs]
GO
/****** Object:  Table [dbo].[AdminSessions]    Script Date: 4/3/2026 11:59:58 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AdminSessions]') AND type in (N'U'))
DROP TABLE [dbo].[AdminSessions]
GO
/****** Object:  Table [dbo].[Admins]    Script Date: 4/3/2026 11:59:58 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admins]') AND type in (N'U'))
DROP TABLE [dbo].[Admins]
GO
/****** Object:  Table [dbo].[AdminRoles]    Script Date: 4/3/2026 11:59:58 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AdminRoles]') AND type in (N'U'))
DROP TABLE [dbo].[AdminRoles]
GO
/****** Object:  Table [dbo].[AdminActivityLogs]    Script Date: 4/3/2026 11:59:58 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AdminActivityLogs]') AND type in (N'U'))
DROP TABLE [dbo].[AdminActivityLogs]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 4/3/2026 11:59:58 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type in (N'U'))
DROP TABLE [dbo].[__EFMigrationsHistory]
GO
USE [master]
GO
/****** Object:  Database [RankUp_AdminDB]    Script Date: 4/3/2026 11:59:58 AM ******/
DROP DATABASE [RankUp_AdminDB]
GO
/****** Object:  Database [RankUp_AdminDB]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE DATABASE [RankUp_AdminDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'RankUp_AdminDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RankUp_AdminDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'RankUp_AdminDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RankUp_AdminDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [RankUp_AdminDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [RankUp_AdminDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [RankUp_AdminDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [RankUp_AdminDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [RankUp_AdminDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [RankUp_AdminDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [RankUp_AdminDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [RankUp_AdminDB] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [RankUp_AdminDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [RankUp_AdminDB] SET  MULTI_USER 
GO
ALTER DATABASE [RankUp_AdminDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [RankUp_AdminDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [RankUp_AdminDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [RankUp_AdminDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [RankUp_AdminDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [RankUp_AdminDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [RankUp_AdminDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [RankUp_AdminDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [RankUp_AdminDB]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdminActivityLogs]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdminActivityLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdminId] [int] NOT NULL,
	[Action] [nvarchar](100) NOT NULL,
	[Resource] [nvarchar](200) NULL,
	[ResourceId] [int] NULL,
	[Details] [nvarchar](1000) NULL,
	[IpAddress] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_AdminActivityLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdminRoles]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdminRoles](
	[AdminId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[Id] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_AdminRoles] PRIMARY KEY CLUSTERED 
(
	[AdminId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Admins]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admins](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Role] [nvarchar](50) NOT NULL,
	[IsTwoFactorEnabled] [bit] NOT NULL,
	[TwoFactorSecret] [nvarchar](max) NULL,
	[RefreshToken] [nvarchar](max) NULL,
	[RefreshTokenExpiryTime] [datetime2](7) NULL,
	[LastLoginAt] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Admins] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdminSessions]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdminSessions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdminId] [int] NOT NULL,
	[Token] [nvarchar](max) NOT NULL,
	[RefreshToken] [nvarchar](max) NOT NULL,
	[ExpiresAt] [datetime2](7) NOT NULL,
	[DeviceInfo] [nvarchar](max) NULL,
	[IpAddress] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_AdminSessions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditLogs]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DashboardCaches]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DashboardCaches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CacheKey] [nvarchar](100) NOT NULL,
	[CacheData] [nvarchar](max) NOT NULL,
	[ExpiresAt] [datetime2](7) NULL,
	[CacheType] [nvarchar](50) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_DashboardCaches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExportLogs]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ExportLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permissions]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permissions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Resource] [nvarchar](200) NULL,
	[Action] [nvarchar](50) NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePermissions]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermissions](
	[RoleId] [int] NOT NULL,
	[PermissionId] [int] NOT NULL,
	[Id] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_RolePermissions] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[PermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260114083816_InitialCreate', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260115130149_AddAuditLogExportLogDashboardCache', N'8.0.0')
GO
SET IDENTITY_INSERT [dbo].[Admins] ON 

INSERT [dbo].[Admins] ([Id], [UserId], [Role], [IsTwoFactorEnabled], [TwoFactorSecret], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (1, 1, N'Admin', 0, NULL, NULL, NULL, NULL, CAST(N'2026-03-10T08:50:19.3766667' AS DateTime2), CAST(N'2026-03-10T08:50:19.3766667' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[Admins] OFF
GO
/****** Object:  Index [IX_AdminActivityLogs_AdminId_CreatedAt]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE NONCLUSTERED INDEX [IX_AdminActivityLogs_AdminId_CreatedAt] ON [dbo].[AdminActivityLogs]
(
	[AdminId] ASC,
	[CreatedAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AdminRoles_RoleId]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE NONCLUSTERED INDEX [IX_AdminRoles_RoleId] ON [dbo].[AdminRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Admins_UserId]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE NONCLUSTERED INDEX [IX_Admins_UserId] ON [dbo].[Admins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AdminSessions_AdminId]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE NONCLUSTERED INDEX [IX_AdminSessions_AdminId] ON [dbo].[AdminSessions]
(
	[AdminId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AuditLogs_AdminId_CreatedAt]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_AdminId_CreatedAt] ON [dbo].[AuditLogs]
(
	[AdminId] ASC,
	[CreatedAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuditLogs_ServiceName]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_ServiceName] ON [dbo].[AuditLogs]
(
	[ServiceName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_DashboardCaches_CacheKey]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_DashboardCaches_CacheKey] ON [dbo].[DashboardCaches]
(
	[CacheKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_DashboardCaches_CacheType]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE NONCLUSTERED INDEX [IX_DashboardCaches_CacheType] ON [dbo].[DashboardCaches]
(
	[CacheType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_DashboardCaches_ExpiresAt]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE NONCLUSTERED INDEX [IX_DashboardCaches_ExpiresAt] ON [dbo].[DashboardCaches]
(
	[ExpiresAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExportLogs_AdminId]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE NONCLUSTERED INDEX [IX_ExportLogs_AdminId] ON [dbo].[ExportLogs]
(
	[AdminId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ExportLogs_ExportType]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE NONCLUSTERED INDEX [IX_ExportLogs_ExportType] ON [dbo].[ExportLogs]
(
	[ExportType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExportLogs_Status]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE NONCLUSTERED INDEX [IX_ExportLogs_Status] ON [dbo].[ExportLogs]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Permissions_Name_Resource_Action]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Permissions_Name_Resource_Action] ON [dbo].[Permissions]
(
	[Name] ASC,
	[Resource] ASC,
	[Action] ASC
)
WHERE ([Resource] IS NOT NULL AND [Action] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RolePermissions_PermissionId]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE NONCLUSTERED INDEX [IX_RolePermissions_PermissionId] ON [dbo].[RolePermissions]
(
	[PermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Roles_Name]    Script Date: 4/3/2026 11:59:58 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Roles_Name] ON [dbo].[Roles]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AdminActivityLogs] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[AdminRoles] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[AdminRoles] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[Admins] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Admins] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[AdminSessions] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[AdminSessions] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[AuditLogs] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[DashboardCaches] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ExportLogs] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Permissions] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Permissions] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[RolePermissions] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[RolePermissions] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[Roles] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Roles] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[AdminActivityLogs]  WITH CHECK ADD  CONSTRAINT [FK_AdminActivityLogs_Admins_AdminId] FOREIGN KEY([AdminId])
REFERENCES [dbo].[Admins] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdminActivityLogs] CHECK CONSTRAINT [FK_AdminActivityLogs_Admins_AdminId]
GO
ALTER TABLE [dbo].[AdminRoles]  WITH CHECK ADD  CONSTRAINT [FK_AdminRoles_Admins_AdminId] FOREIGN KEY([AdminId])
REFERENCES [dbo].[Admins] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdminRoles] CHECK CONSTRAINT [FK_AdminRoles_Admins_AdminId]
GO
ALTER TABLE [dbo].[AdminRoles]  WITH CHECK ADD  CONSTRAINT [FK_AdminRoles_Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdminRoles] CHECK CONSTRAINT [FK_AdminRoles_Roles_RoleId]
GO
ALTER TABLE [dbo].[AdminSessions]  WITH CHECK ADD  CONSTRAINT [FK_AdminSessions_Admins_AdminId] FOREIGN KEY([AdminId])
REFERENCES [dbo].[Admins] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdminSessions] CHECK CONSTRAINT [FK_AdminSessions_Admins_AdminId]
GO
ALTER TABLE [dbo].[AuditLogs]  WITH CHECK ADD  CONSTRAINT [FK_AuditLogs_Admins_AdminId] FOREIGN KEY([AdminId])
REFERENCES [dbo].[Admins] ([Id])
GO
ALTER TABLE [dbo].[AuditLogs] CHECK CONSTRAINT [FK_AuditLogs_Admins_AdminId]
GO
ALTER TABLE [dbo].[ExportLogs]  WITH CHECK ADD  CONSTRAINT [FK_ExportLogs_Admins_AdminId] FOREIGN KEY([AdminId])
REFERENCES [dbo].[Admins] ([Id])
GO
ALTER TABLE [dbo].[ExportLogs] CHECK CONSTRAINT [FK_ExportLogs_Admins_AdminId]
GO
ALTER TABLE [dbo].[RolePermissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permissions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RolePermissions] CHECK CONSTRAINT [FK_RolePermissions_Permissions_PermissionId]
GO
ALTER TABLE [dbo].[RolePermissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RolePermissions] CHECK CONSTRAINT [FK_RolePermissions_Roles_RoleId]
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_Create]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_Create]
    @SubjectId INT,
    @LanguageId INT,
    @Name NVARCHAR(200),
    @IsActive BIT = 1,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO SubjectLanguages (SubjectId, LanguageId, Name, IsActive, CreatedAt, UpdatedAt)
        OUTPUT INSERTED.Id
        VALUES (@SubjectId, @LanguageId, @Name, @IsActive, @CreatedAt, @UpdatedAt);
        
        RETURN 0; -- Success
    END TRY
    BEGIN CATCH
        RETURN ERROR_NUMBER();
    END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_Delete]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE SubjectLanguages 
        SET IsActive = 0, UpdatedAt = GETUTCDATE()
        WHERE Id = @Id;
        
        RETURN 0; -- Success
    END TRY
    BEGIN CATCH
        RETURN ERROR_NUMBER();
    END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_GetById]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT sl.Id, sl.SubjectId, sl.LanguageId, sl.Name, sl.IsActive, sl.CreatedAt, sl.UpdatedAt,
           l.Id as LanguageId, l.Code as LanguageCode, l.Name as LanguageName
    FROM SubjectLanguages sl WITH (NOLOCK)
    INNER JOIN Languages l WITH (NOLOCK) ON sl.LanguageId = l.Id
    WHERE sl.Id = @Id AND sl.IsActive = 1;
END

GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_GetBySubjectId]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_GetBySubjectId]
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT sl.Id, sl.SubjectId, sl.LanguageId, sl.Name, sl.IsActive, sl.CreatedAt, sl.UpdatedAt,
           l.Id as LanguageId, l.Code as LanguageCode, l.Name as LanguageName
    FROM SubjectLanguages sl WITH (NOLOCK)
    INNER JOIN Languages l WITH (NOLOCK) ON sl.LanguageId = l.Id
    WHERE sl.SubjectId = @SubjectId AND sl.IsActive = 1
    ORDER BY l.Name;
END

GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_Update]    Script Date: 4/3/2026 11:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_Update]
    @Id INT,
    @SubjectId INT,
    @LanguageId INT,
    @Name NVARCHAR(200),
    @IsActive BIT = 1,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE SubjectLanguages 
        SET SubjectId = @SubjectId, LanguageId = @LanguageId, Name = @Name, 
            IsActive = @IsActive, UpdatedAt = @UpdatedAt
        WHERE Id = @Id;
        
        RETURN 0; -- Success
    END TRY
    BEGIN CATCH
        RETURN ERROR_NUMBER();
    END CATCH
END

GO
USE [master]
GO
ALTER DATABASE [RankUp_AdminDB] SET  READ_WRITE 
GO
