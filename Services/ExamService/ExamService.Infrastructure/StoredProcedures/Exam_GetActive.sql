CREATE PROCEDURE [dbo].[Exam_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            Id, Name, Description, IsActive, DisplayOrder,
            CreatedAt, UpdatedAt
        FROM Exams
        WHERE IsActive = 1
        ORDER BY DisplayOrder;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
