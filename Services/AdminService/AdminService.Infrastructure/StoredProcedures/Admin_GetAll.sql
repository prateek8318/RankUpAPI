CREATE PROCEDURE [dbo].[Admin_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            Id, UserId, Name, Email, PhoneNumber, IsActive,
            CreatedAt, UpdatedAt
        FROM Admins
        ORDER BY CreatedAt DESC;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
