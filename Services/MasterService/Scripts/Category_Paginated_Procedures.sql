-- Paginated stored procedures for Category table

-- Category_GetAllPaginated
CREATE OR ALTER PROCEDURE [dbo].[Category_GetAllPaginated]
    @Skip INT = 0,
    @Take INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM [dbo].[Categories];
    
    -- Get paginated categories
    SELECT 
        c.Id,
        c.NameEn,
        c.NameHi,
        c.[Key],
        c.Type,
        c.Description,
        c.DisplayOrder,
        c.IsActive,
        c.CreatedAt,
        c.UpdatedAt
    FROM [dbo].[Categories] c
    ORDER BY c.DisplayOrder, c.Id
    OFFSET @Skip ROWS
    FETCH NEXT @Take ROWS ONLY;
END

-- Category_GetActivePaginated
CREATE OR ALTER PROCEDURE [dbo].[Category_GetActivePaginated]
    @Skip INT = 0,
    @Take INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM [dbo].[Categories]
    WHERE IsActive = 1;
    
    -- Get paginated active categories
    SELECT 
        c.Id,
        c.NameEn,
        c.NameHi,
        c.[Key],
        c.Type,
        c.Description,
        c.DisplayOrder,
        c.IsActive,
        c.CreatedAt,
        c.UpdatedAt
    FROM [dbo].[Categories] c
    WHERE c.IsActive = 1
    ORDER BY c.DisplayOrder, c.Id
    OFFSET @Skip ROWS
    FETCH NEXT @Take ROWS ONLY;
END
