CREATE PROCEDURE [dbo].[Exam_GetByQualificationAndStream]
    @QualificationId INT,
    @StreamId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT DISTINCT
            e.Id, e.Name, e.Description, e.IsActive, e.DisplayOrder,
            e.CreatedAt, e.UpdatedAt
        FROM Exams e
        INNER JOIN ExamQualifications eq ON e.Id = eq.ExamId
        WHERE eq.QualificationId = @QualificationId
            AND e.IsActive = 1
            AND (@StreamId IS NULL OR eq.StreamId = @StreamId)
        ORDER BY e.DisplayOrder;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
