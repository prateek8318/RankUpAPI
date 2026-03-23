USE [RankUp_MasterDB]
GO

PRINT 'Creating Category Toggle Status Procedure...';
PRINT '====================================================';

-- Category_ToggleStatus
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_ToggleStatus]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Category_ToggleStatus]
GO

CREATE PROCEDURE [dbo].[Category_ToggleStatus]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if category exists
    IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = @Id)
    BEGIN
        RAISERROR('Category not found', 16, 1);
        RETURN -1;
    END
    
    UPDATE Categories
    SET 
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
    
    RETURN 0;
END
GO

PRINT '====================================================';
PRINT 'CATEGORY TOGGLE STATUS PROCEDURE CREATED SUCCESSFULLY!';
PRINT '====================================================';
GO
