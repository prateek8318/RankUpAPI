USE [RankUp_MasterDB]
GO

PRINT 'Creating Missing Stored Procedures...';
PRINT '====================================================';

-- Stream_SetActive (Missing - causing status toggle to fail)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_SetActive]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Stream_SetActive]
GO

CREATE PROCEDURE [dbo].[Stream_SetActive]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if stream exists
    IF NOT EXISTS (SELECT 1 FROM Streams WHERE Id = @Id)
    BEGIN
        RAISERROR('Stream not found', 16, 1);
        RETURN -1;
    END
    
    UPDATE Streams
    SET 
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
    
    RETURN 0;
END
GO

-- Qualification_SetActive (Missing - causing status toggle to fail)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_SetActive]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Qualification_SetActive]
GO

CREATE PROCEDURE [dbo].[Qualification_SetActive]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if qualification exists
    IF NOT EXISTS (SELECT 1 FROM Qualifications WHERE Id = @Id)
    BEGIN
        RAISERROR('Qualification not found', 16, 1);
        RETURN -1;
    END
    
    UPDATE Qualifications
    SET 
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
    
    RETURN 0;
END
GO

-- Add any other missing SetActive procedures here if needed

PRINT '====================================================';
PRINT 'MISSING STORED PROCEDURES CREATED SUCCESSFULLY!';
PRINT '====================================================';
GO
