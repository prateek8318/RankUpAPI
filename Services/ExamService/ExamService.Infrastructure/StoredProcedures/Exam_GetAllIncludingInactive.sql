CREATE PROCEDURE [dbo].[Exam_GetAllIncludingInactive]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        DurationInMinutes,
        TotalMarks,
        PassingMarks,
        ImageUrl,
        IsInternational,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM 
        [dbo].[Exams]
    ORDER BY 
        Name;
END
