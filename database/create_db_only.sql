-- Create RankUp_UserDB Database
CREATE DATABASE [RankUp_UserDB];
GO

USE [RankUp_UserDB];
GO

-- Create Users Table
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
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO

-- Create UserSocialLogins Table
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
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO

-- Create Foreign Key Constraint
ALTER TABLE [dbo].[UserSocialLogins]  WITH CHECK ADD  CONSTRAINT [FK_UserSocialLogins_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE;
GO

ALTER TABLE [dbo].[UserSocialLogins] CHECK CONSTRAINT [FK_UserSocialLogins_Users_UserId];
GO

-- Create Indexes
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]
(
	[Email] ASC
)
WHERE ([Email] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY];
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_PhoneNumber] ON [dbo].[Users]
(
	[PhoneNumber] ASC
)
WHERE ([PhoneNumber] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY];
GO

CREATE NONCLUSTERED INDEX [IX_UserSocialLogins_UserId] ON [dbo].[UserSocialLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY];
GO

-- Set Default Values
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt];
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive];
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (CONVERT([bit],(0))) FOR [InterestedInIntlExam];
GO

-- Database is ready for use
PRINT 'RankUp_UserDB created successfully!';
GO
