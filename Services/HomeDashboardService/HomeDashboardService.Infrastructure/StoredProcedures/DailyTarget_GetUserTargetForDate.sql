CREATE PROCEDURE DailyTarget_GetUserTargetForDate
    @UserId INT,
    @TargetDate DATETIME2
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
        AND CAST(TargetDate AS DATE) = CAST(@TargetDate AS DATE)
        AND IsActive = 1;
END
