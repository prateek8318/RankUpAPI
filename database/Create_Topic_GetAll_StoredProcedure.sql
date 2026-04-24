USE [RankUp_QuestionDB]
GO

-- Create Topic_GetAll Stored Procedure
CREATE PROCEDURE [dbo].[Topic_GetAll]
    @SubjectId INT = NULL,
    @ExamId INT = NULL,
    @IncludeInactive BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.Id,
        t.Name,
        t.SubjectId,
        t.Description,
        t.ParentTopicId,
        t.SortOrder,
        t.IsActive,
        t.CreatedAt,
        t.UpdatedAt,
        pt.Name AS ParentTopicName
    FROM Topics t
    LEFT JOIN Topics pt ON t.ParentTopicId = pt.Id
    WHERE (@SubjectId IS NULL OR t.SubjectId = @SubjectId)
    AND (@IncludeInactive = 1 OR t.IsActive = 1)
    ORDER BY t.SortOrder, t.Name;
END
GO

PRINT 'Topic_GetAll Stored Procedure Created Successfully'
