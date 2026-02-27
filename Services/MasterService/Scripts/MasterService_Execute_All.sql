

USE [RankUp_MasterDB]
GO

PRINT 'Starting MasterService Stored Procedures Execution...';
PRINT '====================================================';

-- Execute Part 1
PRINT 'Executing Part 1: Language, State, Country Procedures...';
:r "MasterService_StoredProcedures_Part1.sql"
GO

-- Execute Part 2  
PRINT 'Executing Part 2: Qualification, Stream, Subject, Exam Procedures...';
:r "MasterService_StoredProcedures_Part2.sql"
GO

-- Execute Part 3
PRINT 'Executing Part 3: CMS Content & Language Mapping Procedures...';
:r "MasterService_StoredProcedures_Part3.sql"
GO

PRINT '====================================================';
PRINT 'ALL MASTERSERVICE STORED PROCEDURES EXECUTED SUCCESSFULLY!';
PRINT '====================================================';
PRINT 'Your Dapper repositories should now work correctly!';
PRINT '====================================================';
GO

-- =====================================================
-- Verification Script - Check if all procedures exist
-- =====================================================

PRINT 'Verifying stored procedures creation...';
PRINT '====================================================';

-- Check Language procedures
SELECT 'Language_GetById' as ProcedureName, 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language_GetById]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END as Status
UNION ALL
SELECT 'Language_GetAll', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language_GetAll]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Language_GetActive', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language_GetActive]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Language_Create', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language_Create]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Language_Update', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language_Update]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Language_Delete', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language_Delete]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
-- Check State procedures
SELECT 'State_GetById', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_GetById]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'State_GetAll', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_GetAll]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'State_GetActive', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_GetActive]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'State_GetByCountryCode', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_GetByCountryCode]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'State_Create', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_Create]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'State_Update', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_Update]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'State_Delete', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_Delete]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
-- Check Country procedures
SELECT 'Country_GetById', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_GetById]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Country_GetAll', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_GetAll]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Country_GetActive', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_GetActive]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Country_Create', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_Create]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Country_Update', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_Update]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Country_Delete', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_Delete]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
-- Check Qualification procedures
SELECT 'Qualification_GetById', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_GetById]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Qualification_GetAll', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_GetAll]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Qualification_GetActive', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_GetActive]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Qualification_GetByCountryCode', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_GetByCountryCode]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Qualification_Create', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_Create]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Qualification_Update', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_Update]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Qualification_Delete', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_Delete]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
-- Check Stream procedures
SELECT 'Stream_GetById', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_GetById]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Stream_GetAll', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_GetAll]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Stream_GetActive', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_GetActive]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Stream_GetByQualificationId', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_GetByQualificationId]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Stream_Create', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_Create]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Stream_Update', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_Update]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Stream_Delete', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_Delete]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
-- Check Subject procedures
SELECT 'Subject_GetById', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_GetById]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Subject_GetAll', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_GetAll]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Subject_GetActive', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_GetActive]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Subject_Create', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_Create]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Subject_Update', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_Update]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Subject_Delete', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_Delete]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
-- Check Exam procedures
SELECT 'Exam_GetById', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetById]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Exam_GetAll', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetAll]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Exam_GetActive', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetActive]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Exam_GetByCountryCode', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetByCountryCode]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Exam_GetInternational', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetInternational]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Exam_Create', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_Create]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Exam_Update', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_Update]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'Exam_Delete', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_Delete]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
-- Check CMS Content procedures
SELECT 'CmsContent_GetById', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_GetById]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'CmsContent_GetAll', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_GetAll]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'CmsContent_GetActive', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_GetActive]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'CmsContent_GetByType', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_GetByType]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'CmsContent_GetByStatus', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_GetByStatus]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'CmsContent_Create', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_Create]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'CmsContent_Update', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_Update]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
UNION ALL
SELECT 'CmsContent_Delete', 
       CASE WHEN EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_Delete]') AND type in (N'P', N'PC')) THEN 'EXISTS' ELSE 'MISSING' END
ORDER BY ProcedureName;

-- PRINT '====================================================';
-- PRINT 'VERIFICATION COMPLETE!';
-- PRINT 'Check the results above - all should show EXISTS status';
-- PRINT 'If any show MISSING, re-run the corresponding script';
-- PRINT '====================================================';
GO
