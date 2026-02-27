CREATE PROCEDURE DailyTarget_GetUserTargets
    @UserId INT,
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        UserId,
        TargetDate,
        TargetQuestions,
        TargetTimeMinutes,
        QuestionsCompleted,
        TimeSpentMinutes,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM DailyTargets
    WHERE UserId = @UserId 
        AND IsActive = 1
        AND (@StartDate IS NULL OR TargetDate >= @StartDate)
        AND (@EndDate IS NULL OR TargetDate <= @EndDate)
    ORDER BY TargetDate DESC;
END
