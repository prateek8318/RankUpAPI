CREATE PROCEDURE [dbo].[Exam_GetByIdWithQualifications]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            e.Id, 
            e.Name, 
            e.Description, 
            e.DurationInMinutes,
            e.TotalMarks,
            e.PassingMarks,
            e.IsActive, 
            e.ImageUrl,
            e.IsInternational,
            e.CreatedAt, 
            e.UpdatedAt,
            -- Include ExamQualifications as a nested collection
            (
                SELECT 
                    eq.Id,
                    eq.ExamId,
                    eq.QualificationId,
                    eq.StreamId,
                    eq.IsActive,
                    eq.CreatedAt,
                    eq.UpdatedAt
                FROM ExamQualifications eq
                WHERE eq.ExamId = e.Id AND eq.IsActive = 1
                FOR JSON PATH
            ) AS ExamQualifications
        FROM Exams e
        WHERE e.Id = @Id;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
