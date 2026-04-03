-- Manual Database Deployment Instructions
-- ========================================
-- 
-- For manual deployment to your SQL Server instance:
--
-- 1. Connect to your SQL Server instance using SQL Server Management Studio
--    or sqlcmd with appropriate credentials
--
-- 2. Run the following command to execute the database script:
--    sqlcmd -S <your_server_name> -U <username> -P <password> -i database/RankUp_UserDB_Script.sql
--
-- 3. Or copy and execute the contents of database/RankUp_UserDB_Script.sql
--    directly in SQL Server Management Studio
--
-- Connection Details for Docker Setup:
-- Server: localhost,1433
-- Database: RankUp_UserDB
-- Username: sa
-- Password: RankUp@ProdPass2026!
--
-- For Production Server:
-- Update the connection details in your docker/.env file
-- and ensure SQL Server is accessible from your application
--
-- ========================================

-- Quick verification query (run after deployment):
SELECT 
    name AS DatabaseName,
    state_desc AS DatabaseStatus,
    create_date AS CreatedDate
FROM sys.databases 
WHERE name = 'RankUp_UserDB';

-- Check tables in the database:
USE [RankUp_UserDB];
SELECT 
    name AS TableName,
    create_date AS CreatedDate
FROM sys.tables 
WHERE is_ms_shipped = 0
ORDER BY create_date;

-- Check stored procedures:
SELECT 
    name AS ProcedureName,
    create_date AS CreatedDate
FROM sys.procedures 
WHERE is_ms_shipped = 0
ORDER BY create_date;
