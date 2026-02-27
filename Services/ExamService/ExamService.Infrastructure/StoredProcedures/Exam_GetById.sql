CREATE PROCEDURE [dbo].[Exam_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            Id, Name, Description, IsActive, DisplayOrder,
            CreatedAt, UpdatedAt
        FROM Exams
        WHERE Id = @Id;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
