IF COL_LENGTH('dbo.MockTestSessionAnswers', 'IsReported') IS NULL
BEGIN
    ALTER TABLE dbo.MockTestSessionAnswers
    ADD IsReported BIT NOT NULL CONSTRAINT DF_MockTestSessionAnswers_IsReported DEFAULT 0;
END;

IF COL_LENGTH('dbo.MockTestSessionAnswers', 'ReportReason') IS NULL
BEGIN
    ALTER TABLE dbo.MockTestSessionAnswers
    ADD ReportReason NVARCHAR(500) NULL;
END;

IF COL_LENGTH('dbo.MockTestSessionAnswers', 'ReportedAt') IS NULL
BEGIN
    ALTER TABLE dbo.MockTestSessionAnswers
    ADD ReportedAt DATETIME2 NULL;
END;

IF COL_LENGTH('dbo.MockTestSessionAnswers', 'IsBookmarked') IS NULL
BEGIN
    ALTER TABLE dbo.MockTestSessionAnswers
    ADD IsBookmarked BIT NOT NULL CONSTRAINT DF_MockTestSessionAnswers_IsBookmarked DEFAULT 0;
END;

IF COL_LENGTH('dbo.MockTestSessionAnswers', 'BookmarkedAt') IS NULL
BEGIN
    ALTER TABLE dbo.MockTestSessionAnswers
    ADD BookmarkedAt DATETIME2 NULL;
END;
