-- Master script to execute all stored procedures
-- Execute this script to create/update all stored procedures for MasterService

-- Execute Category Stored Procedures
:r "Category_StoredProcedures.sql"
GO

-- Execute Category Toggle Status Procedure
:r "Category_ToggleStatus_Procedure.sql"
GO

-- Execute Country Stored Procedures
:r "Country_StoredProcedures.sql"
GO

-- Execute State Stored Procedures
:r "State_StoredProcedures.sql"
GO

-- Execute Subject Stored Procedures
:r "Subject_StoredProcedures.sql"
GO

-- Execute SubjectLanguage GetBySubjectId Procedure
:r "SubjectLanguage_GetBySubjectId.sql"
GO

-- Execute existing procedures
:r "SubjectLanguageProcedures.sql"
GO

PRINT '====================================================';
PRINT 'ALL STORED PROCEDURES EXECUTED SUCCESSFULLY!';
PRINT '====================================================';
GO
