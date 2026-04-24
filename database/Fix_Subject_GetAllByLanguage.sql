-- Fix for Subject_GetAllByLanguage stored procedure
-- This script will modify the procedure to return all subjects, not just those with language mappings

-- First drop the existing procedure
DROP PROCEDURE [dbo].[Subject_GetAllByLanguage];
GO

-- Create the updated procedure that returns all subjects
CREATE PROCEDURE [dbo].[Subject_GetAllByLanguage]
    @LanguageId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Return all subjects, with language mappings if available
    SELECT 
        s.Id,
        s.Name,
        s.Description,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM [dbo].[Subjects] s
    WHERE s.IsActive = 1
    ORDER BY s.Name;
END
GO

-- Print confirmation
PRINT 'Subject_GetAllByLanguage procedure has been updated to return all subjects.';
GO
