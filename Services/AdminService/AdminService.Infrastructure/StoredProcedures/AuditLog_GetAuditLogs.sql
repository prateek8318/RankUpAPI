CREATE PROCEDURE [dbo].[AuditLog_GetAuditLogs]
    @AdminId INT = NULL,
    @ServiceName NVARCHAR(100) = NULL,
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL,
    @Page INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            al.Id, al.AdminId, al.ServiceName, al.Action, al.EntityType,
            al.EntityId, al.OldValues, al.NewValues, al.IpAddress,
            al.UserAgent, al.CreatedAt,
            a.Name AS AdminName, a.Email AS AdminEmail
        FROM AuditLogs al
        LEFT JOIN Admins a ON al.AdminId = a.Id
        WHERE (@AdminId IS NULL OR al.AdminId = @AdminId)
            AND (@ServiceName IS NULL OR al.ServiceName = @ServiceName)
            AND (@StartDate IS NULL OR al.CreatedAt >= @StartDate)
            AND (@EndDate IS NULL OR al.CreatedAt <= @EndDate)
        ORDER BY al.CreatedAt DESC
        OFFSET ((@Page - 1) * @PageSize) ROWS
        FETCH NEXT @PageSize ROWS ONLY;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
