-- Create all RankUpAPI databases (run once against SQL Server)
-- Example: run in Azure Data Studio / SSMS connected to localhost,1433 (sa / password from docker/.env)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_UserDB') CREATE DATABASE RankUp_UserDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_ExamDB') CREATE DATABASE RankUp_ExamDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_AdminDB') CREATE DATABASE RankUp_AdminDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_QualificationDB') CREATE DATABASE RankUp_QualificationDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_QuestionDB') CREATE DATABASE RankUp_QuestionDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_QuizDB') CREATE DATABASE RankUp_QuizDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_SubscriptionDB') CREATE DATABASE RankUp_SubscriptionDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_PaymentDB') CREATE DATABASE RankUp_PaymentDB;
GO
