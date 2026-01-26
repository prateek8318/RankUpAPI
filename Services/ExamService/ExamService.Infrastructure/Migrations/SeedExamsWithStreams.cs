using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedExamsWithStreams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Get Qualification IDs from QualificationService database
            // Note: These IDs will be determined at runtime, so we use a script that queries the QualificationService
            
            migrationBuilder.Sql(@"
                -- First, delete existing exam-qualification relationships and exams (optional)
                -- DELETE FROM ExamQualifications;
                -- DELETE FROM Exams;

                -- Insert National Exams
                -- Note: Qualification IDs need to be fetched from QualificationService
                -- This is a template - actual IDs will be resolved when migration runs

                DECLARE @Qual10th INT;
                DECLARE @Qual12thScience INT;
                DECLARE @Qual12thCommerce INT;
                DECLARE @Qual12thArts INT;
                DECLARE @QualGradScience INT;
                DECLARE @QualGradCommerce INT;
                DECLARE @QualGradArts INT;
                DECLARE @QualGradGeneral INT;
                DECLARE @QualPGScience INT;
                DECLARE @QualPGGeneral INT;

                DECLARE @StreamScience INT;
                DECLARE @StreamCommerce INT;
                DECLARE @StreamArts INT;
                DECLARE @StreamGeneral INT;

                -- Get Stream IDs (assuming they exist in QualificationService)
                -- Note: In a microservices architecture, we store IDs only
                -- These values should match the Stream IDs from QualificationService
                SET @StreamScience = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Streams] WHERE Name = 'Science');
                SET @StreamCommerce = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Streams] WHERE Name = 'Commerce');
                SET @StreamArts = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Streams] WHERE Name = 'Arts');
                SET @StreamGeneral = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Streams] WHERE Name = 'General');

                -- Get Qualification IDs
                SET @Qual10th = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Qualifications] WHERE Name = '10th Grade');
                SET @Qual12thScience = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Qualifications] WHERE Name = '12th Grade - Science');
                SET @Qual12thCommerce = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Qualifications] WHERE Name = '12th Grade - Commerce');
                SET @Qual12thArts = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Qualifications] WHERE Name = '12th Grade - Arts');
                SET @QualGradScience = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Qualifications] WHERE Name = 'Graduation - Science');
                SET @QualGradCommerce = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Qualifications] WHERE Name = 'Graduation - Commerce');
                SET @QualGradArts = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Qualifications] WHERE Name = 'Graduation - Arts');
                SET @QualGradGeneral = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Qualifications] WHERE Name = 'Graduation - General');
                SET @QualPGScience = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Qualifications] WHERE Name = 'Post Graduation - Science');
                SET @QualPGGeneral = (SELECT TOP 1 Id FROM [RankUp_QualificationDB].[dbo].[Qualifications] WHERE Name = 'Post Graduation - General');

                -- National Exams for 12th Science
                IF @Qual12thScience IS NOT NULL AND NOT EXISTS (SELECT * FROM Exams WHERE Name = 'JEE Main' AND IsInternational = 0)
                BEGIN
                    DECLARE @JeeMainId INT;
                    INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
                    VALUES ('JEE Main', 'Joint Entrance Examination Main for Engineering', 180, 300, 100, '/images/exams/jee-main.jpg', 0, 1, GETDATE());
                    SET @JeeMainId = SCOPE_IDENTITY();
                    
                    INSERT INTO ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt)
                    VALUES (@JeeMainId, @Qual12thScience, @StreamScience, 1, GETDATE());
                END

                IF @Qual12thScience IS NOT NULL AND NOT EXISTS (SELECT * FROM Exams WHERE Name = 'JEE Advanced' AND IsInternational = 0)
                BEGIN
                    DECLARE @JeeAdvancedId INT;
                    INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
                    VALUES ('JEE Advanced', 'Joint Entrance Examination Advanced for IITs', 180, 360, 120, '/images/exams/jee-advanced.jpg', 0, 1, GETDATE());
                    SET @JeeAdvancedId = SCOPE_IDENTITY();
                    
                    INSERT INTO ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt)
                    VALUES (@JeeAdvancedId, @Qual12thScience, @StreamScience, 1, GETDATE());
                END

                IF @Qual12thScience IS NOT NULL AND NOT EXISTS (SELECT * FROM Exams WHERE Name = 'NEET' AND IsInternational = 0)
                BEGIN
                    DECLARE @NeetId INT;
                    INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
                    VALUES ('NEET', 'National Eligibility cum Entrance Test for Medical', 180, 720, 240, '/images/exams/neet.jpg', 0, 1, GETDATE());
                    SET @NeetId = SCOPE_IDENTITY();
                    
                    INSERT INTO ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt)
                    VALUES (@NeetId, @Qual12thScience, @StreamScience, 1, GETDATE());
                END

                -- National Exams for Graduation General
                IF @QualGradGeneral IS NOT NULL AND NOT EXISTS (SELECT * FROM Exams WHERE Name = 'UPSC Civil Services' AND IsInternational = 0)
                BEGIN
                    DECLARE @UpscId INT;
                    INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
                    VALUES ('UPSC Civil Services', 'Union Public Service Commission Civil Services Examination', 180, 1000, 350, '/images/exams/upsc.jpg', 0, 1, GETDATE());
                    SET @UpscId = SCOPE_IDENTITY();
                    
                    INSERT INTO ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt)
                    VALUES (@UpscId, @QualGradGeneral, @StreamGeneral, 1, GETDATE());
                END

                IF @QualGradGeneral IS NOT NULL AND NOT EXISTS (SELECT * FROM Exams WHERE Name = 'SSC CGL' AND IsInternational = 0)
                BEGIN
                    DECLARE @SscCglId INT;
                    INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
                    VALUES ('SSC CGL', 'Staff Selection Commission Combined Graduate Level', 120, 200, 70, '/images/exams/ssc-cgl.jpg', 0, 1, GETDATE());
                    SET @SscCglId = SCOPE_IDENTITY();
                    
                    INSERT INTO ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt)
                    VALUES (@SscCglId, @QualGradGeneral, @StreamGeneral, 1, GETDATE());
                END

                IF @QualGradGeneral IS NOT NULL AND NOT EXISTS (SELECT * FROM Exams WHERE Name = 'Banking PO' AND IsInternational = 0)
                BEGIN
                    DECLARE @BankingPoId INT;
                    INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
                    VALUES ('Banking PO', 'Banking Probationary Officer Examination', 120, 200, 70, '/images/exams/banking-po.jpg', 0, 1, GETDATE());
                    SET @BankingPoId = SCOPE_IDENTITY();
                    
                    INSERT INTO ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt)
                    VALUES (@BankingPoId, @QualGradGeneral, @StreamGeneral, 1, GETDATE());
                END

                -- International Exams for 12th Science
                IF @Qual12thScience IS NOT NULL AND NOT EXISTS (SELECT * FROM Exams WHERE Name = 'SAT' AND IsInternational = 1)
                BEGIN
                    DECLARE @SatId INT;
                    INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
                    VALUES ('SAT', 'Scholastic Assessment Test for US Universities', 180, 1600, 1000, '/images/exams/sat.jpg', 1, 1, GETDATE());
                    SET @SatId = SCOPE_IDENTITY();
                    
                    INSERT INTO ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt)
                    VALUES (@SatId, @Qual12thScience, @StreamScience, 1, GETDATE());
                END

                IF @Qual12thScience IS NOT NULL AND NOT EXISTS (SELECT * FROM Exams WHERE Name = 'IELTS Academic' AND IsInternational = 1)
                BEGIN
                    DECLARE @IeltsAcademicId INT;
                    INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
                    VALUES ('IELTS Academic', 'International English Language Testing System - Academic', 165, 9, 6.5, '/images/exams/ielts-academic.jpg', 1, 1, GETDATE());
                    SET @IeltsAcademicId = SCOPE_IDENTITY();
                    
                    INSERT INTO ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt)
                    VALUES (@IeltsAcademicId, @Qual12thScience, @StreamScience, 1, GETDATE());
                END

                IF @Qual12thScience IS NOT NULL AND NOT EXISTS (SELECT * FROM Exams WHERE Name = 'TOEFL' AND IsInternational = 1)
                BEGIN
                    DECLARE @ToeflId INT;
                    INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
                    VALUES ('TOEFL', 'Test of English as a Foreign Language', 180, 120, 80, '/images/exams/toefl.jpg', 1, 1, GETDATE());
                    SET @ToeflId = SCOPE_IDENTITY();
                    
                    INSERT INTO ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt)
                    VALUES (@ToeflId, @Qual12thScience, @StreamScience, 1, GETDATE());
                END

                -- International Exams for Graduation General
                IF @QualGradGeneral IS NOT NULL AND NOT EXISTS (SELECT * FROM Exams WHERE Name = 'GRE' AND IsInternational = 1)
                BEGIN
                    DECLARE @GreId INT;
                    INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
                    VALUES ('GRE', 'Graduate Record Examination for US Graduate Schools', 210, 340, 260, '/images/exams/gre.jpg', 1, 1, GETDATE());
                    SET @GreId = SCOPE_IDENTITY();
                    
                    INSERT INTO ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt)
                    VALUES (@GreId, @QualGradGeneral, @StreamGeneral, 1, GETDATE());
                END

                IF @QualGradGeneral IS NOT NULL AND NOT EXISTS (SELECT * FROM Exams WHERE Name = 'GMAT' AND IsInternational = 1)
                BEGIN
                    DECLARE @GmatId INT;
                    INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
                    VALUES ('GMAT', 'Graduate Management Admission Test for Business Schools', 187, 800, 550, '/images/exams/gmat.jpg', 1, 1, GETDATE());
                    SET @GmatId = SCOPE_IDENTITY();
                    
                    INSERT INTO ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt)
                    VALUES (@GmatId, @QualGradGeneral, @StreamGeneral, 1, GETDATE());
                END

                IF @QualGradGeneral IS NOT NULL AND NOT EXISTS (SELECT * FROM Exams WHERE Name = 'PLAB' AND IsInternational = 1)
                BEGIN
                    DECLARE @PlabId INT;
                    INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
                    VALUES ('PLAB', 'Professional and Linguistic Assessments Board for UK Medical Practice', 180, 180, 120, '/images/exams/plab.jpg', 1, 1, GETDATE());
                    SET @PlabId = SCOPE_IDENTITY();
                    
                    INSERT INTO ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt)
                    VALUES (@PlabId, @QualGradGeneral, @StreamGeneral, 1, GETDATE());
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ExamQualifications WHERE ExamId IN (
                    SELECT Id FROM Exams WHERE Name IN (
                        'JEE Main', 'JEE Advanced', 'NEET', 'UPSC Civil Services', 
                        'SSC CGL', 'Banking PO', 'SAT', 'IELTS Academic', 'TOEFL', 
                        'GRE', 'GMAT', 'PLAB'
                    )
                );
                DELETE FROM Exams WHERE Name IN (
                    'JEE Main', 'JEE Advanced', 'NEET', 'UPSC Civil Services', 
                    'SSC CGL', 'Banking PO', 'SAT', 'IELTS Academic', 'TOEFL', 
                    'GRE', 'GMAT', 'PLAB'
                );
            ");
        }
    }
}
