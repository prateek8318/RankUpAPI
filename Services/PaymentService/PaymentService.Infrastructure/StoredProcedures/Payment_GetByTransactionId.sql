CREATE PROCEDURE [dbo].[Payment_GetByTransactionId]
    @TransactionId NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            Id, UserId, TransactionId, Amount, Currency,
            PaymentMethod, Status, GatewayTransactionId,
            GatewayResponse, CreatedAt, UpdatedAt, IsActive
        FROM Payments
        WHERE TransactionId = @TransactionId;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
