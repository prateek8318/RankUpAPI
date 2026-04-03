USE [RankUp_UserDB]
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_Update]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[UserSocialLogin_Update]
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_Insert]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[UserSocialLogin_Insert]
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_GetByUserIdAndProvider]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[UserSocialLogin_GetByUserIdAndProvider]
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_GetByUserId]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[UserSocialLogin_GetByUserId]
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_GetByProviderAndProviderId]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[UserSocialLogin_GetByProviderAndProviderId]
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_GetByProviderAndGoogleId]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[UserSocialLogin_GetByProviderAndGoogleId]
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_GetById]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[UserSocialLogin_GetById]
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_GetByEmail]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[UserSocialLogin_GetByEmail]
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_Delete]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[UserSocialLogin_Delete]
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_Create]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[UserSocialLogin_Create]
GO
/****** Object:  StoredProcedure [dbo].[User_Update]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[User_Update]
GO
/****** Object:  StoredProcedure [dbo].[User_GetTotalCount]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[User_GetTotalCount]
GO
/****** Object:  StoredProcedure [dbo].[User_GetDailyActiveCount]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[User_GetDailyActiveCount]
GO
/****** Object:  StoredProcedure [dbo].[User_GetByPhoneNumber]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[User_GetByPhoneNumber]
GO
/****** Object:  StoredProcedure [dbo].[User_GetById_Basic]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[User_GetById_Basic]
GO
/****** Object:  StoredProcedure [dbo].[User_GetById]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[User_GetById]
GO
/****** Object:  StoredProcedure [dbo].[User_GetByGoogleId]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[User_GetByGoogleId]
GO
/****** Object:  StoredProcedure [dbo].[User_GetByEmail]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[User_GetByEmail]
GO
/****** Object:  StoredProcedure [dbo].[User_GetAll]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[User_GetAll]
GO
/****** Object:  StoredProcedure [dbo].[User_Delete]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[User_Delete]
GO
/****** Object:  StoredProcedure [dbo].[User_Create]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[User_Create]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_Update]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[Qualification_Update]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetById]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[Qualification_GetById]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetActive]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[Qualification_GetActive]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_Create]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP PROCEDURE [dbo].[Qualification_Create]
GO
ALTER TABLE [dbo].[UserSocialLogins] DROP CONSTRAINT [FK_UserSocialLogins_Users_UserId]
GO
ALTER TABLE [dbo].[Users] DROP CONSTRAINT [DF__Users__Intereste__690797E6]
GO
ALTER TABLE [dbo].[Users] DROP CONSTRAINT [DF__Users__IsActive__681373AD]
GO
ALTER TABLE [dbo].[Users] DROP CONSTRAINT [DF__Users__CreatedAt__671F4F74]
GO
/****** Object:  Index [IX_UserSocialLogins_UserId]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP INDEX [IX_UserSocialLogins_UserId] ON [dbo].[UserSocialLogins]
GO
/****** Object:  Index [IX_Users_PhoneNumber]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP INDEX [IX_Users_PhoneNumber] ON [dbo].[Users]
GO
/****** Object:  Index [IX_Users_Email]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP INDEX [IX_Users_Email] ON [dbo].[Users]
GO
/****** Object:  Table [dbo].[UserSocialLogins]    Script Date: 4/3/2026 10:56:02 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSocialLogins]') AND type in (N'U'))
DROP TABLE [dbo].[UserSocialLogins]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 4/3/2026 10:56:02 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
DROP TABLE [dbo].[Users]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 4/3/2026 10:56:02 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type in (N'U'))
DROP TABLE [dbo].[__EFMigrationsHistory]
GO
USE [master]
GO
/****** Object:  Database [RankUp_UserDB]    Script Date: 4/3/2026 10:56:02 AM ******/
DROP DATABASE [RankUp_UserDB]
GO
/****** Object:  Database [RankUp_UserDB]    Script Date: 4/3/2026 10:56:03 AM ******/
CREATE DATABASE [RankUp_UserDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'RankUp_UserDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RankUp_UserDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'RankUp_UserDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RankUp_UserDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [RankUp_UserDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [RankUp_UserDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [RankUp_UserDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [RankUp_UserDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [RankUp_UserDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [RankUp_UserDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET QUOTED_IDENTIFIER ON 
GO
ALTER DATABASE [RankUp_UserDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [RankUp_UserDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [RankUp_UserDB] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [RankUp_UserDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [RankUp_UserDB] SET  MULTI_USER 
GO
ALTER DATABASE [RankUp_UserDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [RankUp_UserDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [RankUp_UserDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [RankUp_UserDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [RankUp_UserDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [RankUp_UserDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [RankUp_UserDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [RankUp_UserDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [RankUp_UserDB]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 4/3/2026 10:56:03 AM ******/
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
/****** Object:  Table [dbo].[Users]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[PasswordHash] [nvarchar](255) NULL,
	[PhoneNumber] [nvarchar](15) NULL,
	[Gender] [nvarchar](20) NULL,
	[DateOfBirth] [date] NULL,
	[Qualification] [nvarchar](100) NULL,
	[ProfilePhoto] [nvarchar](255) NULL,
	[PreferredExam] [nvarchar](100) NULL,
	[StateId] [int] NULL,
	[LanguageId] [int] NULL,
	[QualificationId] [int] NULL,
	[ExamId] [int] NULL,
	[RefreshToken] [nvarchar](max) NULL,
	[RefreshTokenExpiryTime] [datetime2](7) NULL,
	[LastLoginAt] [datetime2](7) NULL,
	[IsPhoneVerified] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[CountryCode] [nvarchar](10) NULL,
	[InterestedInIntlExam] [bit] NOT NULL,
	[PreferredLanguage] [nvarchar](5) NULL,
	[CategoryId] [int] NULL,
	[StreamId] [int] NULL,
	[DeviceId] [nvarchar](100) NULL,
	[DeviceType] [nvarchar](50) NULL,
	[DeviceName] [nvarchar](100) NULL,
	[FcmToken] [nvarchar](500) NULL,
	[LastDeviceLoginAt] [datetime2](7) NULL,
	[LastDeviceType] [nvarchar](50) NULL,
	[LastDeviceName] [nvarchar](100) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserSocialLogins]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSocialLogins](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Provider] [nvarchar](max) NULL,
	[GoogleId] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[AvatarUrl] [nvarchar](max) NULL,
	[AccessToken] [nvarchar](max) NULL,
	[RefreshToken] [nvarchar](max) NULL,
	[ExpiresAt] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_UserSocialLogins] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260116134239_CheckSchema', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260116134823_AddCountryCodeToUser', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260122094004_AddInternationalExamFields', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260122095813_RemoveDuplicateInternationalExamColumn', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260209060538_UpdateLanguagePreferenceToPreferredLanguage', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260209083338_AddCategoryAndStreamToUser', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260223102258_InitialCreateBaseline', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260223102447_RemoveObsoleteUserColumns', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260223102542_DropPasswordHashColumn', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260223125150_AddGoogleIdToUsers', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260224170857_AddUserSocialLoginsTable', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260224172444_FixUserSocialLoginsNullableColumns', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260225050306_FixUserIdIdentity', N'8.0.0')
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (1, N'User', NULL, NULL, N'+912255555888', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-25T05:45:22.2300000' AS DateTime2), 1, CAST(N'2026-03-25T05:45:22.1966667' AS DateTime2), CAST(N'2026-03-25T05:45:22.2366667' AS DateTime2), 1, N'+91', 0, N'en', NULL, NULL, N'UP1A.231005.007', N'android', N'SM-A042F', N'ewkwqBW5Twe1-t4TB7vYCD:APA91bFYoc1fATvvbU4y_mTHrC2imOo-lCDPxjBNGZCiUvW_ebXKm2ix-BG9p0cOe9ydw092iM-9U5LHdnoM8aF0mr1O0-H9En6VMHjAdEM2KY8X2HGEKj8', CAST(N'2026-03-25T05:45:22.2366667' AS DateTime2), N'android', N'SM-A042F')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (2, N'User', N'Hshsh@sjjs.snsnn', NULL, N'+918987787784', N'Male', CAST(N'2000-01-01' AS Date), NULL, N'uploads/profiles/user_2_20260325070740.jpg', NULL, 67, 50, 4, NULL, NULL, NULL, CAST(N'2026-03-25T06:49:56.1066667' AS DateTime2), 1, CAST(N'2026-03-25T06:49:56.0266667' AS DateTime2), CAST(N'2026-03-25T07:07:40.6100000' AS DateTime2), 1, N'+91', 1, N'en', 7, NULL, N'BP2A.250605.031.A3', N'android', N'SM-M055F', N'csAMyK4UROyacsMNGHcOes:APA91bGcKyYOmrlxNwisyu8CopSimtbfXiNXTGxpMqetNT7Ak7-kKNBqWxhEtNXOv_MF4T2-iCz-MXkRnj04TCjxrOGYDOErmCAFiZbm844Vnyux2Wcz9r0', CAST(N'2026-03-25T06:49:56.1133333' AS DateTime2), N'android', N'SM-M055F')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (3, N'Tekniko Gkobal', N'teknikogkobal@gmail.com', NULL, N'8851445555', N'Male', CAST(N'2000-01-01' AS Date), NULL, NULL, NULL, 67, 50, 4, NULL, NULL, NULL, CAST(N'2026-03-25T09:23:05.6066667' AS DateTime2), 0, CAST(N'2026-03-25T08:21:30.2800000' AS DateTime2), CAST(N'2026-03-25T09:23:33.3166667' AS DateTime2), 1, N'+91', 1, N'en', 7, NULL, N'BP2A.250605.031.A3', N'android', N'SM-M055F', NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (4, N'Testnew', N'vibhas9926@gmail.com', NULL, N'+919311620055', N'Male', CAST(N'2000-01-01' AS Date), NULL, N'uploads/profiles/user_4_20260325082636.jpg', NULL, 69, 50, 4, NULL, NULL, NULL, CAST(N'2026-03-25T08:23:43.2966667' AS DateTime2), 1, CAST(N'2026-03-25T08:23:43.2766667' AS DateTime2), CAST(N'2026-03-25T08:26:36.9400000' AS DateTime2), 1, N'+91', 1, N'en', 7, NULL, N'UKQ1.230924.001', N'android', N'CPH2381', N'cSFg2fbrTAaoGlZ7mF_UFb:APA91bGQcigh2V8V5tNAy09wjNm413LiGeOXsF955NUHwwTk9XlISpJidw-Zj_qmNIBBo-FfoOV8to3lnyBX6zW6SUQ5IHEe-naEdDphQYwMSGr6Gxc3TeM', CAST(N'2026-03-25T08:23:43.3000000' AS DateTime2), N'android', N'CPH2381')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (5, N'Userhshs', N'Hshs@gshs.com', NULL, N'+919799797979', N'Male', CAST(N'2000-01-01' AS Date), NULL, N'uploads/profiles/user_5_20260325082754.jpg', NULL, 67, 50, 4, NULL, NULL, NULL, CAST(N'2026-03-25T08:35:05.5266667' AS DateTime2), 1, CAST(N'2026-03-25T08:27:06.6133333' AS DateTime2), CAST(N'2026-03-25T08:35:05.5433333' AS DateTime2), 1, N'+91', 1, N'en', 7, NULL, N'BP2A.250605.031.A3', N'android', N'SM-M055F', N'csAMyK4UROyacsMNGHcOes:APA91bGcKyYOmrlxNwisyu8CopSimtbfXiNXTGxpMqetNT7Ak7-kKNBqWxhEtNXOv_MF4T2-iCz-MXkRnj04TCjxrOGYDOErmCAFiZbm844Vnyux2Wcz9r0', CAST(N'2026-03-25T08:35:05.5433333' AS DateTime2), N'android', N'SM-M055F')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (6, N'User', NULL, NULL, N'+919311620024', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-25T08:36:02.5266667' AS DateTime2), 1, CAST(N'2026-03-25T08:36:02.5066667' AS DateTime2), CAST(N'2026-03-25T08:36:02.5300000' AS DateTime2), 1, N'+91', 0, N'en', NULL, NULL, N'UKQ1.230924.001', N'android', N'CPH2381', N'c5KtxGVIS3iUJwDu7qqbvo:APA91bEx1M2TR_coel_wn4pH4fakuChvUp7XaX2NWREM928MKlqcrWapdgo5jkSjCHOMEQxWxPbtOcIICXoxFlAHKIxi5IRk_dqm9TnS53XYBxOTQhlFEfs', CAST(N'2026-03-25T08:36:02.5300000' AS DateTime2), N'android', N'CPH2381')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (7, N'User', NULL, NULL, N'+915656565656', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-25T08:39:20.7566667' AS DateTime2), 1, CAST(N'2026-03-25T08:39:20.7366667' AS DateTime2), CAST(N'2026-03-25T08:39:20.7600000' AS DateTime2), 1, N'+91', 0, N'en', NULL, NULL, N'UKQ1.230924.001', N'android', N'CPH2381', N'cV4YFtDqSAGIMJoBB6b3bK:APA91bG00N0y106RUdUJpF3mWEx63_toT6CZbE5CRuSNGxDsN3X_Lz7us2behxQHiZPIMrlbvjErQp9UU5qkKq9czmr9mlGXt45CaYwz-h4Q77OHbHH33eg', CAST(N'2026-03-25T08:39:20.7600000' AS DateTime2), N'android', N'CPH2381')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (8, N'User', NULL, NULL, N'+919595959595', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-25T08:50:58.4766667' AS DateTime2), 1, CAST(N'2026-03-25T08:50:58.4566667' AS DateTime2), CAST(N'2026-03-25T08:50:58.4800000' AS DateTime2), 1, N'+91', 0, N'en', NULL, NULL, N'UKQ1.230924.001', N'android', N'CPH2381', N'fAKnJAjdQ86xIqhf1l59iA:APA91bGnas1bCfrb4FPZeSFz0cH7o-e6VZwPcuIkfmv245auCqh1gZ7DYrmAZZNMq5ay6Lwl_eSUJtw72RzitMvDBw1jLTxSpI9aWp3Jrtab-Mn4NFW1nMw', CAST(N'2026-03-25T08:50:58.4800000' AS DateTime2), N'android', N'CPH2381')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (9, N'User', NULL, NULL, N'+919494949494', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-25T08:56:07.7566667' AS DateTime2), 1, CAST(N'2026-03-25T08:56:07.7033333' AS DateTime2), CAST(N'2026-03-25T08:56:07.7666667' AS DateTime2), 1, N'+91', 0, N'en', NULL, NULL, N'UKQ1.230924.001', N'android', N'CPH2381', N'f3RIWeisQga27Q5bVGnU4t:APA91bGETvoYwcOWiSxhz7Y1K-FrGRQsl3nykcOU7Vt_J1tKKs1RYGj59vn6QEpQpb6h9dUrz47Zf3l4Av-7U4QQK2AGWfMlVVatoRobATBWo5Hilpk3lKo', CAST(N'2026-03-25T08:56:07.7666667' AS DateTime2), N'android', N'CPH2381')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (10, N'User', NULL, NULL, N'+915865858585', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-25T08:58:18.8733333' AS DateTime2), 1, CAST(N'2026-03-25T08:58:18.8533333' AS DateTime2), CAST(N'2026-03-25T08:58:18.8800000' AS DateTime2), 1, N'+91', 0, N'en', NULL, NULL, N'BP2A.250605.031.A3', N'android', N'SM-M055F', N'csAMyK4UROyacsMNGHcOes:APA91bGcKyYOmrlxNwisyu8CopSimtbfXiNXTGxpMqetNT7Ak7-kKNBqWxhEtNXOv_MF4T2-iCz-MXkRnj04TCjxrOGYDOErmCAFiZbm844Vnyux2Wcz9r0', CAST(N'2026-03-25T08:58:18.8800000' AS DateTime2), N'android', N'SM-M055F')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (11, N'User', NULL, NULL, N'+919311620022', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-25T09:00:09.4000000' AS DateTime2), 1, CAST(N'2026-03-25T09:00:09.3800000' AS DateTime2), CAST(N'2026-03-25T09:00:09.4033333' AS DateTime2), 1, N'+91', 0, N'en', NULL, NULL, N'UKQ1.230924.001', N'android', N'CPH2381', N'ezQBgPBGTRyMRHnqS7puhZ:APA91bEvpnOkMhUJwgNqZf4o9KvfZTEarHAqUwuxZ6t5_F9SIvZtGcBNC22jGZfH7RBVqw3UUfmNSzMFI9-efud69FXVtAvnWMlVxzo9KA8BlpJ7u_eImls', CAST(N'2026-03-25T09:00:09.4033333' AS DateTime2), N'android', N'CPH2381')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (12, N'Rama', N'rama@gmail.com', NULL, N'+919311620029', N'Female', CAST(N'1996-01-01' AS Date), NULL, N'uploads/profiles/user_12_20260325090449.jpg', NULL, 65, 50, 4, NULL, NULL, NULL, CAST(N'2026-03-25T11:31:21.3000000' AS DateTime2), 1, CAST(N'2026-03-25T09:04:00.4733333' AS DateTime2), CAST(N'2026-03-25T11:31:21.3000000' AS DateTime2), 1, N'+91', 0, N'en', 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (13, N'Testt', N'v@g.com', NULL, N'+915623232323', N'Male', CAST(N'2000-01-01' AS Date), NULL, NULL, NULL, 69, 50, 6, NULL, NULL, NULL, CAST(N'2026-03-25T09:07:11.9733333' AS DateTime2), 1, CAST(N'2026-03-25T09:07:11.9566667' AS DateTime2), CAST(N'2026-03-25T09:07:56.2100000' AS DateTime2), 1, N'+91', 1, N'en', 26, NULL, N'UKQ1.230924.001', N'android', N'CPH2381', N'dBtXrAYyR7aPrmvKFnT16C:APA91bEcfudGbzpgsOJe1kxN84_ngnAjzKIQOR62gwI1T-fZT0coFD8JjBpv6icmHFqI3XCj9lMl6nM0fUh-FBKBiJTCzkykZ65k1_oHzIKUtnq8Ve7nj_I', CAST(N'2026-03-25T09:07:11.9766667' AS DateTime2), N'android', N'CPH2381')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (14, N'User', N'Fhhfhf@fjfj.cjch', NULL, N'+915854655875', N'Male', CAST(N'2000-01-01' AS Date), NULL, NULL, NULL, 67, 49, 4, NULL, NULL, NULL, CAST(N'2026-03-25T09:09:42.4633333' AS DateTime2), 1, CAST(N'2026-03-25T09:09:42.4400000' AS DateTime2), CAST(N'2026-03-25T09:10:15.8733333' AS DateTime2), 1, N'+91', 1, N'en', 2, NULL, N'BP2A.250605.031.A3', N'android', N'SM-M055F', N'c_mwA9nnQ16v3XoMMblLp_:APA91bHblMc4B4OzSFXs6XJ9bG57r_tu8c3zVaoRNti_ep6YjNU4nvyGDsNTUTFNIxPj96bI5RWXPWenfKvD3J_FBeuNfSBkXRfbCDocctKr93LNtZGw5gY', CAST(N'2026-03-25T09:09:42.4666667' AS DateTime2), N'android', N'SM-M055F')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (15, N'Fhhfh', N'g@c.com', NULL, N'+916565656565', N'Male', CAST(N'1994-01-01' AS Date), NULL, NULL, NULL, 65, 49, 4, NULL, NULL, NULL, CAST(N'2026-03-25T09:58:25.1500000' AS DateTime2), 1, CAST(N'2026-03-25T09:58:25.1266667' AS DateTime2), CAST(N'2026-03-25T10:01:11.4766667' AS DateTime2), 1, N'+91', 1, N'en', 2, NULL, N'UKQ1.230924.001', N'android', N'CPH2381', N'fY-2-0KcRkuPoyJuAyxvec:APA91bHMd1-a3ArzbbQ0eXFlZop2dHAOJ4j1MswQx_XwhsM7rfC_61JLqxEyDpu1XXgG0Y0ng-HKjreQLb6EgubtL9FT8xy3nFGvUbsPKuznsBy11FatZww', CAST(N'2026-03-25T09:58:25.1533333' AS DateTime2), N'android', N'CPH2381')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (16, N'Riten Singh', N'riten@gmail.com', NULL, N'+918146657032', N'Male', CAST(N'1988-01-19' AS Date), NULL, N'uploads/profiles/user_16_20260325104713.jpg', NULL, 65, 50, 4, NULL, NULL, NULL, CAST(N'2026-03-25T10:44:06.7033333' AS DateTime2), 1, CAST(N'2026-03-25T10:44:06.6366667' AS DateTime2), CAST(N'2026-03-25T10:49:11.6500000' AS DateTime2), 1, N'+91', 1, N'en', 7, NULL, N'UKQ1.230924.001', N'android', N'CPH2381', N'fY7_Z24nSi-3nzFiDAtt0V:APA91bE3r5C_IISHpRnbhg2twgOpiLVmRrAvxaPcp_zD1c1EaV57FvTrPPiuQBOj07gdXRc0XZBcznsH3I-Qyzmz6rF_ObAzI6knwjT1laatgrud4ZKLpt4', CAST(N'2026-03-25T10:44:06.7066667' AS DateTime2), N'android', N'CPH2381')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (17, N'John Doe Updated', N'john.updated12@example.com', NULL, N'+917894561231', N'Male', CAST(N'1990-01-01' AS Date), NULL, N'uploads/profiles/user_17_20260325112303.jpg', NULL, 1, 1, 1, 1, NULL, NULL, CAST(N'2026-03-25T11:22:34.9800000' AS DateTime2), 1, CAST(N'2026-03-25T11:07:04.9333333' AS DateTime2), CAST(N'2026-03-25T11:23:03.9966667' AS DateTime2), 1, N'+91', 0, N'en', 1, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (19, N'User', NULL, NULL, N'+918686686898', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-25T12:08:42.4033333' AS DateTime2), 1, CAST(N'2026-03-25T12:08:42.2933333' AS DateTime2), CAST(N'2026-03-25T12:08:42.4233333' AS DateTime2), 1, N'+91', 0, NULL, NULL, NULL, N'BP2A.250605.031.A3', N'android', N'SM-M055F', N'c_mwA9nnQ16v3XoMMblLp_:APA91bHblMc4B4OzSFXs6XJ9bG57r_tu8c3zVaoRNti_ep6YjNU4nvyGDsNTUTFNIxPj96bI5RWXPWenfKvD3J_FBeuNfSBkXRfbCDocctKr93LNtZGw5gY', CAST(N'2026-03-25T12:08:42.4233333' AS DateTime2), N'android', N'SM-M055F')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (20, N'test', N'john.updated22@example.com', NULL, N'+918888888888', N'Male', CAST(N'1990-01-01' AS Date), NULL, N'uploads/profiles/user_20_20260325121746.jpg', NULL, 1, 1, 1, 1, NULL, NULL, CAST(N'2026-03-25T12:16:41.9700000' AS DateTime2), 1, CAST(N'2026-03-25T12:16:41.9166667' AS DateTime2), CAST(N'2026-03-25T12:17:46.4066667' AS DateTime2), 1, N'+91', 0, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (21, N'Usercgyggu', N'Uffufu@yffu.uvgu', NULL, N'+916526266262', N'Male', CAST(N'2000-01-01' AS Date), NULL, N'uploads/profiles/user_21_20260325121808.jpg', NULL, 67, 50, 4, NULL, NULL, NULL, CAST(N'2026-03-25T12:17:07.3933333' AS DateTime2), 1, CAST(N'2026-03-25T12:17:07.3900000' AS DateTime2), CAST(N'2026-03-25T12:18:08.4033333' AS DateTime2), 1, N'+91', 1, NULL, 2, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (22, N'User', NULL, NULL, N'+918888888882', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-25T12:23:10.5000000' AS DateTime2), 1, CAST(N'2026-03-25T12:23:10.4466667' AS DateTime2), CAST(N'2026-03-25T12:23:10.5000000' AS DateTime2), 1, N'+91', 0, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (23, N'', NULL, NULL, N'+918888888884', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-25T13:17:57.7366667' AS DateTime2), 1, CAST(N'2026-03-25T13:11:50.4433333' AS DateTime2), CAST(N'2026-03-25T13:17:57.7366667' AS DateTime2), 1, N'+91', 0, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (24, N'', NULL, NULL, N'+918888888885', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-25T13:18:18.9033333' AS DateTime2), 1, CAST(N'2026-03-25T13:18:18.7400000' AS DateTime2), CAST(N'2026-03-25T13:18:18.9033333' AS DateTime2), 1, N'+91', 0, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (25, N'', NULL, NULL, N'+919999999999', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-25T13:28:30.9700000' AS DateTime2), 1, CAST(N'2026-03-25T13:28:30.6766667' AS DateTime2), CAST(N'2026-03-25T13:28:30.9733333' AS DateTime2), 1, N'+91', 0, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (26, N'', NULL, NULL, N'+918888888887', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-26T05:10:27.9700000' AS DateTime2), 1, CAST(N'2026-03-25T13:32:26.4333333' AS DateTime2), CAST(N'2026-03-26T05:10:27.9700000' AS DateTime2), 1, N'+91', 0, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (27, N'', NULL, NULL, N'+918888888825', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-26T05:10:53.8033333' AS DateTime2), 1, CAST(N'2026-03-26T05:10:53.7733333' AS DateTime2), CAST(N'2026-03-26T05:10:53.8033333' AS DateTime2), 1, N'+91', 0, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (28, N'', NULL, NULL, N'+918888888823', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-26T05:14:18.5766667' AS DateTime2), 1, CAST(N'2026-03-26T05:14:18.5200000' AS DateTime2), CAST(N'2026-03-26T05:14:18.5766667' AS DateTime2), 1, N'+91', 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (29, N'Fyfyfytugufu', N'Jsjs@jsjsj.sxkkd', NULL, N'+919559262662', N'पुरुष', CAST(N'2000-01-09' AS Date), NULL, N'uploads/profiles/user_29_20260326071257.jpg', NULL, 67, 49, 4, NULL, NULL, NULL, CAST(N'2026-03-26T09:34:37.3366667' AS DateTime2), 1, CAST(N'2026-03-26T06:42:30.5500000' AS DateTime2), CAST(N'2026-03-26T09:38:09.1233333' AS DateTime2), 1, N'+91', 1, NULL, 2, 7, N'BP2A.250605.031.A3', N'android', N'SM-M055F', N'dHeGzMXoQJm3VdAt4YFXp6:APA91bFi6EcZe2vunHlnRYDF3jLxjNkHCOeduUy7DeiP77fHk11FNtTM5YBC26a2PMm6yxNCpcze6o0dorV-v2eMfQ2bPoS_dNSkJ2_ce2hJz28Rjp4LiEg', CAST(N'2026-03-26T09:31:23.1300000' AS DateTime2), N'android', N'SM-M055F')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (30, N'Raj', N'Djjs@jsjs.snjs', NULL, N'9632545455', N'Male', CAST(N'2000-01-01' AS Date), NULL, N'uploads/profiles/user_30_20260327071648.jpg', NULL, 67, 50, 4, NULL, NULL, NULL, CAST(N'2026-03-27T07:14:41.8433333' AS DateTime2), 1, CAST(N'2026-03-27T07:14:41.7866667' AS DateTime2), CAST(N'2026-03-27T07:16:48.2800000' AS DateTime2), 1, N'+91', 1, NULL, 2, 7, N'BP2A.250605.031.A3', N'android', N'SM-M055F', N'dHeGzMXoQJm3VdAt4YFXp6:APA91bFi6EcZe2vunHlnRYDF3jLxjNkHCOeduUy7DeiP77fHk11FNtTM5YBC26a2PMm6yxNCpcze6o0dorV-v2eMfQ2bPoS_dNSkJ2_ce2hJz28Rjp4LiEg', CAST(N'2026-03-27T07:14:41.8466667' AS DateTime2), N'android', N'SM-M055F')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (31, N'', NULL, NULL, N'+917894561230', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-30T05:10:35.6900000' AS DateTime2), 1, CAST(N'2026-03-27T13:35:59.9266667' AS DateTime2), CAST(N'2026-03-30T05:10:35.7266667' AS DateTime2), 1, N'+91', 0, NULL, NULL, NULL, N'54215421', N'android', N'Samsung', N'jsdksfdhdsjkl', CAST(N'2026-03-30T05:10:35.7266667' AS DateTime2), N'android', N'Samsung')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (32, N'Vibhas Mishra', N'vibhads1326@gmail.com', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-30T05:11:26.1133333' AS DateTime2), 0, CAST(N'2026-03-30T05:11:26.0866667' AS DateTime2), CAST(N'2026-03-30T05:11:26.1133333' AS DateTime2), 1, N'+91', 0, NULL, NULL, NULL, N'UKQ1.230924.001', N'android', N'CPH2381', NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (33, N'Hydhdh', N'Shhs@sjjs.snnsn', NULL, N'9654545455', N'Male', CAST(N'2000-01-01' AS Date), NULL, N'uploads/profiles/user_33_20260330051634.jpg', NULL, 67, 50, 1, NULL, NULL, NULL, CAST(N'2026-03-30T05:15:47.1500000' AS DateTime2), 1, CAST(N'2026-03-30T05:15:47.0966667' AS DateTime2), CAST(N'2026-03-30T05:16:34.2433333' AS DateTime2), 1, N'+91', 1, NULL, 2, 7, N'BP2A.250605.031.A3', N'android', N'SM-M055F', N'dHeGzMXoQJm3VdAt4YFXp6:APA91bFi6EcZe2vunHlnRYDF3jLxjNkHCOeduUy7DeiP77fHk11FNtTM5YBC26a2PMm6yxNCpcze6o0dorV-v2eMfQ2bPoS_dNSkJ2_ce2hJz28Rjp4LiEg', CAST(N'2026-03-30T05:15:47.1566667' AS DateTime2), N'android', N'SM-M055F')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (34, N'', NULL, NULL, N'+918484848489', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2026-03-30T06:54:26.1100000' AS DateTime2), 1, CAST(N'2026-03-30T06:54:26.0433333' AS DateTime2), CAST(N'2026-03-30T06:54:26.1166667' AS DateTime2), 1, N'+91', 0, NULL, NULL, NULL, N'BP2A.250605.031.A3', N'android', N'SM-M055F', N'dHeGzMXoQJm3VdAt4YFXp6:APA91bFi6EcZe2vunHlnRYDF3jLxjNkHCOeduUy7DeiP77fHk11FNtTM5YBC26a2PMm6yxNCpcze6o0dorV-v2eMfQ2bPoS_dNSkJ2_ce2hJz28Rjp4LiEg', CAST(N'2026-03-30T06:54:26.1166667' AS DateTime2), N'android', N'SM-M055F')
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [PhoneNumber], [Gender], [DateOfBirth], [Qualification], [ProfilePhoto], [PreferredExam], [StateId], [LanguageId], [QualificationId], [ExamId], [RefreshToken], [RefreshTokenExpiryTime], [LastLoginAt], [IsPhoneVerified], [CreatedAt], [UpdatedAt], [IsActive], [CountryCode], [InterestedInIntlExam], [PreferredLanguage], [CategoryId], [StreamId], [DeviceId], [DeviceType], [DeviceName], [FcmToken], [LastDeviceLoginAt], [LastDeviceType], [LastDeviceName]) VALUES (35, N'Fbfhfh', N'Fjfjjffjfj@fjfjgj.gjgj', NULL, N'+915575855858', N'Male', CAST(N'2000-01-01' AS Date), NULL, N'uploads/profiles/user_35_20260330073410.jpg', NULL, 67, 50, 1, NULL, NULL, NULL, CAST(N'2026-03-30T07:33:30.7200000' AS DateTime2), 1, CAST(N'2026-03-30T07:33:30.6666667' AS DateTime2), CAST(N'2026-03-30T07:34:10.0733333' AS DateTime2), 1, N'+91', 1, NULL, 2, 6, N'BP2A.250605.031.A3', N'android', N'SM-M055F', N'dHeGzMXoQJm3VdAt4YFXp6:APA91bFi6EcZe2vunHlnRYDF3jLxjNkHCOeduUy7DeiP77fHk11FNtTM5YBC26a2PMm6yxNCpcze6o0dorV-v2eMfQ2bPoS_dNSkJ2_ce2hJz28Rjp4LiEg', CAST(N'2026-03-30T07:33:30.7233333' AS DateTime2), N'android', N'SM-M055F')
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET IDENTITY_INSERT [dbo].[UserSocialLogins] ON 

INSERT [dbo].[UserSocialLogins] ([Id], [UserId], [Provider], [GoogleId], [Email], [Name], [AvatarUrl], [AccessToken], [RefreshToken], [ExpiresAt], [CreatedAt], [UpdatedAt]) VALUES (2, 3, N'Google', N'Xr0hs3YOTbXOi8svAuIc0CfPiiE2', N'teknikogkobal@gmail.com', N'Tekniko Gkobal', NULL, NULL, NULL, NULL, CAST(N'2026-03-25T14:53:05.5933333' AS DateTime2), CAST(N'2026-03-25T14:53:05.5933333' AS DateTime2))
INSERT [dbo].[UserSocialLogins] ([Id], [UserId], [Provider], [GoogleId], [Email], [Name], [AvatarUrl], [AccessToken], [RefreshToken], [ExpiresAt], [CreatedAt], [UpdatedAt]) VALUES (3, 32, N'Google', N'7caMUXJyqAOLC4saENmkokwXza45', N'vibhads1326@gmail.com', N'Vibhas Mishra', NULL, NULL, NULL, NULL, CAST(N'2026-03-30T10:41:26.1300000' AS DateTime2), CAST(N'2026-03-30T10:41:26.1300000' AS DateTime2))
SET IDENTITY_INSERT [dbo].[UserSocialLogins] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_Email]    Script Date: 4/3/2026 10:56:03 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]
(
	[Email] ASC
)
WHERE ([Email] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_PhoneNumber]    Script Date: 4/3/2026 10:56:03 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_PhoneNumber] ON [dbo].[Users]
(
	[PhoneNumber] ASC
)
WHERE ([PhoneNumber] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserSocialLogins_UserId]    Script Date: 4/3/2026 10:56:03 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserSocialLogins_UserId] ON [dbo].[UserSocialLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (CONVERT([bit],(0))) FOR [InterestedInIntlExam]
GO
ALTER TABLE [dbo].[UserSocialLogins]  WITH CHECK ADD  CONSTRAINT [FK_UserSocialLogins_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserSocialLogins] CHECK CONSTRAINT [FK_UserSocialLogins_Users_UserId]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_Create]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_Create]
    @Name        NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @CountryCode NVARCHAR(10),
    @IsActive    BIT,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @CreatedAt   DATETIME2,
    @UpdatedAt   DATETIME2,
    @Id          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Qualifications (Name, Description, CountryCode, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Description, @CountryCode, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();

    IF @NamesJson IS NOT NULL AND @Id > 0
    BEGIN
        INSERT INTO dbo.QualificationLanguages (QualificationId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
        SELECT
            @Id as QualificationId,
            LanguageId,
            Name,
            Description,
            1 as IsActive,
            @CreatedAt as CreatedAt,
            @UpdatedAt as UpdatedAt
        FROM OPENJSON(@NamesJson)
        WITH (
            LanguageId INT '$.LanguageId',
            Name NVARCHAR(200) '$.Name',
            Description NVARCHAR(1000) '$.Description'
        );
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetActive]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, NameHi, Description, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Qualifications
    WHERE IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetById]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, NameHi, Description, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Qualifications
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_Update]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_Update]
    @Id          INT,
    @Name        NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @CountryCode NVARCHAR(10),
    @IsActive    BIT,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @UpdatedAt   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Qualifications
    SET Name = @Name, Description = @Description, CountryCode = @CountryCode, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;

    IF @NamesJson IS NOT NULL
    BEGIN
        DELETE FROM dbo.QualificationLanguages WHERE QualificationId = @Id;

        INSERT INTO dbo.QualificationLanguages (QualificationId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
        SELECT
            @Id as QualificationId,
            LanguageId,
            Name,
            Description,
            1 as IsActive,
            @UpdatedAt as CreatedAt,
            @UpdatedAt as UpdatedAt
        FROM OPENJSON(@NamesJson)
        WITH (
            LanguageId INT '$.LanguageId',
            Name NVARCHAR(200) '$.Name',
            Description NVARCHAR(1000) '$.Description'
        );
    END
END
GO
/****** Object:  StoredProcedure [dbo].[User_Create]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[User_Create]
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(15),
    @PasswordHash NVARCHAR(255),
    @ProfilePhoto NVARCHAR(255),
    @IsActive BIT = 1,
    @PreferredLanguage NVARCHAR(5) = 'en',
    @IsPhoneVerified BIT = 0,
    @InterestedInIntlExam BIT = 0,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Users] (
        Name, Email, PhoneNumber, PasswordHash, ProfilePhoto, 
        IsActive, PreferredLanguage, IsPhoneVerified, InterestedInIntlExam,
        CreatedAt, UpdatedAt
    )
    VALUES (
        @Name, @Email, @PhoneNumber, @PasswordHash, @ProfilePhoto, 
        @IsActive, @PreferredLanguage, @IsPhoneVerified, @InterestedInIntlExam,
        @CreatedAt, @UpdatedAt
    );
    
    SET @UserId = SCOPE_IDENTITY();
END

GO
/****** Object:  StoredProcedure [dbo].[User_Delete]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[User_Delete]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Users] 
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @UserId;
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetAll]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[User_GetAll]
    @Page INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@Page - 1) * @PageSize;
    
    SELECT 
        Id,
        Name,
        Email,
        PhoneNumber,
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
        LastLoginAt,
        IsPhoneVerified,
        InterestedInIntlExam,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Users] 
    WHERE IsActive = 1
    ORDER BY CreatedAt DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetByEmail]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[User_GetByEmail]
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Email,
        PhoneNumber,
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
        LastLoginAt,
        IsPhoneVerified,
        InterestedInIntlExam,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Users] 
    WHERE Email = @Email AND IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetByGoogleId]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[User_GetByGoogleId]
    @GoogleId NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Email,
        PhoneNumber,
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
        GoogleId,
        IsPhoneVerified,
        InterestedInIntlExam,
        IsActive,
        CreatedAt,
        UpdatedAt,
        LastLoginAt
    FROM [Users]
    WHERE GoogleId = @GoogleId;
END;

GO
/****** Object:  StoredProcedure [dbo].[User_GetById]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[User_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.Id,
        u.Name,
        u.Email,
        u.PhoneNumber,
        u.CountryCode,
        u.Gender,
        u.DateOfBirth,
        u.Qualification,
        u.PreferredLanguage,
        u.ProfilePhoto,
        u.PreferredExam,
        u.StateId,
        s.Name AS StateName,
        NULL AS StateNameHi,
        u.LanguageId,
        l.Name AS LanguageName,
        NULL AS LanguageNameHi,
        u.QualificationId,
        q.Name AS QualificationName,
        q.NameHi AS QualificationNameHi,
        u.ExamId,
        e.Name AS ExamName,
        NULL AS ExamNameHi,
        u.CategoryId,
        c.NameEn AS CategoryName,
        c.NameHi AS CategoryNameHi,
        u.StreamId,
        st.Name AS StreamName,
        st.NameHi AS StreamNameHi,
        u.RefreshToken,
        u.RefreshTokenExpiryTime,
        u.LastLoginAt,
        u.IsPhoneVerified,
        u.InterestedInIntlExam,
        u.IsActive,
        u.CreatedAt,
        u.UpdatedAt
    FROM [dbo].[Users] u
    LEFT JOIN [RankUp_MasterDB].[dbo].[States] s ON u.StateId = s.Id
    LEFT JOIN [RankUp_MasterDB].[dbo].[Languages] l ON u.LanguageId = l.Id
    LEFT JOIN [RankUp_MasterDB].[dbo].[Qualifications] q ON u.QualificationId = q.Id
    LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON u.ExamId = e.Id
    LEFT JOIN [RankUp_MasterDB].[dbo].[Categories] c ON u.CategoryId = c.Id
    LEFT JOIN [RankUp_MasterDB].[dbo].[Streams] st ON u.StreamId = st.Id
    WHERE u.Id = @Id AND u.IsActive = 1;
END

GO
/****** Object:  StoredProcedure [dbo].[User_GetById_Basic]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[User_GetById_Basic]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Email,
        PasswordHash,
        PhoneNumber,
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
        LastLoginAt,
        IsPhoneVerified,
        InterestedInIntlExam,
        IsActive,
        CreatedAt,
        UpdatedAt,
        DeviceId,
        DeviceType,
        DeviceName,
        FcmToken,
        LastDeviceLoginAt,
        LastDeviceType,
        LastDeviceName
    FROM [dbo].[Users] 
    WHERE Id = @Id AND IsActive = 1;
END

GO
/****** Object:  StoredProcedure [dbo].[User_GetByPhoneNumber]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[User_GetByPhoneNumber]
    @PhoneNumber NVARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Email,
        PhoneNumber,
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
        LastLoginAt,
        IsPhoneVerified,
        InterestedInIntlExam,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Users] 
    WHERE PhoneNumber = @PhoneNumber AND IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetDailyActiveCount]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[User_GetDailyActiveCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) as DailyActiveCount
    FROM [dbo].[Users] 
    WHERE IsActive = 1 
    AND CAST(LastLoginAt AS DATE) = CAST(GETDATE() AS DATE);
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetTotalCount]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[User_GetTotalCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) as TotalCount
    FROM [dbo].[Users] 
    WHERE IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[User_Update]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[User_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(15),
    @CountryCode NVARCHAR(10),
    @Gender NVARCHAR(20),
    @DateOfBirth DATE,
    @Qualification NVARCHAR(100),
    @PreferredLanguage NVARCHAR(5),
    @ProfilePhoto NVARCHAR(255),
    @PreferredExam NVARCHAR(100),
    @StateId INT,
    @LanguageId INT,
    @QualificationId INT,
    @ExamId INT,
    @CategoryId INT,
    @StreamId INT,
    @RefreshToken NVARCHAR(500),
    @RefreshTokenExpiryTime DATETIME,
    @IsPhoneVerified BIT,
    @InterestedInIntlExam BIT,
    @IsActive BIT,
    @UpdatedAt DATETIME,
    @LastLoginAt DATETIME,
    @DeviceId NVARCHAR(100),
    @DeviceType NVARCHAR(50),
    @DeviceName NVARCHAR(100),
    @FcmToken NVARCHAR(500),
    @LastDeviceLoginAt DATETIME,
    @LastDeviceType NVARCHAR(50),
    @LastDeviceName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Users] 
    SET Name = @Name,
        Email = @Email,
        PhoneNumber = @PhoneNumber,
        CountryCode = @CountryCode,
        Gender = @Gender,
        DateOfBirth = @DateOfBirth,
        Qualification = @Qualification,
        PreferredLanguage = @PreferredLanguage,
        ProfilePhoto = @ProfilePhoto,
        PreferredExam = @PreferredExam,
        StateId = @StateId,
        LanguageId = @LanguageId,
        QualificationId = @QualificationId,
        ExamId = @ExamId,
        CategoryId = @CategoryId,
        StreamId = @StreamId,
        RefreshToken = @RefreshToken,
        RefreshTokenExpiryTime = @RefreshTokenExpiryTime,
        IsPhoneVerified = @IsPhoneVerified,
        InterestedInIntlExam = @InterestedInIntlExam,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt,
        LastLoginAt = @LastLoginAt,
        DeviceId = @DeviceId,
        DeviceType = @DeviceType,
        DeviceName = @DeviceName,
        FcmToken = @FcmToken,
        LastDeviceLoginAt = @LastDeviceLoginAt,
        LastDeviceType = @LastDeviceType,
        LastDeviceName = @LastDeviceName
    WHERE Id = @Id;
END

GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_Create]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_Create]
    @UserId INT,
    @Provider NVARCHAR(50),
    @GoogleId NVARCHAR(255),
    @Email NVARCHAR(100),
    @Name NVARCHAR(100),
    @AvatarUrl NVARCHAR(500),
    @AccessToken NVARCHAR(1000),
    @RefreshToken NVARCHAR(1000),
    @ExpiresAt DATETIME,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[UserSocialLogins] (
        UserId, Provider, GoogleId, Email, Name, AvatarUrl,
        AccessToken, RefreshToken, ExpiresAt, CreatedAt, UpdatedAt
    )
    VALUES (
        @UserId, @Provider, @GoogleId, @Email, @Name, @AvatarUrl,
        @AccessToken, @RefreshToken, @ExpiresAt, @CreatedAt, @UpdatedAt
    );
    
    SET @Id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_Delete]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM [dbo].[UserSocialLogins] 
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_GetByEmail]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_GetByEmail]
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        UserId,
        Provider,
        GoogleId,
        Email,
        Name,
        AvatarUrl,
        AccessToken,
        RefreshToken,
        ExpiresAt,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[UserSocialLogins] 
    WHERE Email = @Email;
END
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_GetById]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        UserId,
        Provider,
        GoogleId,
        Email,
        Name,
        AvatarUrl,
        AccessToken,
        RefreshToken,
        ExpiresAt,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[UserSocialLogins] 
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_GetByProviderAndGoogleId]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_GetByProviderAndGoogleId]
    @Provider NVARCHAR(50),
    @GoogleId NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            Id,
            UserId,
            Provider,
            GoogleId,
            Email,
            Name,
            AccessToken,
            RefreshToken,
            ExpiresAt,
            CreatedAt,
            UpdatedAt
        FROM [dbo].[UserSocialLogins]
        WHERE Provider = @Provider 
        AND GoogleId = @GoogleId;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_GetByProviderAndProviderId]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_GetByProviderAndProviderId]
    @Provider NVARCHAR(50),
    @ProviderId NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        UserId,
        Provider,
        GoogleId,
        Email,
        Name,
        AvatarUrl,
        AccessToken,
        RefreshToken,
        ExpiresAt,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[UserSocialLogins] 
    WHERE Provider = @Provider 
    AND (@Provider = 'Google' AND GoogleId = @ProviderId);
END
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_GetByUserId]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_GetByUserId]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        UserId,
        Provider,
        GoogleId,
        Email,
        Name,
        AvatarUrl,
        AccessToken,
        RefreshToken,
        ExpiresAt,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[UserSocialLogins] 
    WHERE UserId = @UserId;
END
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_GetByUserIdAndProvider]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_GetByUserIdAndProvider]
    @UserId INT,
    @Provider NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        UserId,
        Provider,
        GoogleId,
        Email,
        Name,
        AvatarUrl,
        AccessToken,
        RefreshToken,
        ExpiresAt,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[UserSocialLogins] 
    WHERE UserId = @UserId 
    AND Provider = @Provider;
END

GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_Insert]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_Insert]
    @UserId INT,
    @Provider NVARCHAR(50),
    @GoogleId NVARCHAR(255),
    @AvatarUrl NVARCHAR(500),
    @Email NVARCHAR(100),
    @Name NVARCHAR(100),
    @AccessToken NVARCHAR(500),
    @RefreshToken NVARCHAR(500),
    @ExpiresAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[UserSocialLogins] (
        UserId, Provider, GoogleId, AvatarUrl, Email, Name, 
        AccessToken, RefreshToken, ExpiresAt, CreatedAt, UpdatedAt
    )
    VALUES (
        @UserId, @Provider, @GoogleId, @AvatarUrl, @Email, @Name,
        @AccessToken, @RefreshToken, @ExpiresAt, GETDATE(), GETDATE()
    );
    
    SELECT SCOPE_IDENTITY() AS Id;
END
GO
/****** Object:  StoredProcedure [dbo].[UserSocialLogin_Update]    Script Date: 4/3/2026 10:56:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_Update]
    @Id INT,
    @UserId INT,
    @Provider NVARCHAR(50),
    @GoogleId NVARCHAR(255),
    @Email NVARCHAR(100),
    @Name NVARCHAR(100),
    @AvatarUrl NVARCHAR(500),
    @AccessToken NVARCHAR(1000),
    @RefreshToken NVARCHAR(1000),
    @ExpiresAt DATETIME,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[UserSocialLogins] 
    SET UserId = @UserId,
        Provider = @Provider,
        GoogleId = @GoogleId,
        Email = @Email,
        Name = @Name,
        AvatarUrl = @AvatarUrl,
        AccessToken = @AccessToken,
        RefreshToken = @RefreshToken,
        ExpiresAt = @ExpiresAt,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO
USE [master]
GO
ALTER DATABASE [RankUp_UserDB] SET  READ_WRITE 
GO
