-- Generic script to seed international exams for ALL existing qualifications
-- This script automatically finds all qualifications and creates international exams

-- First, let's see what qualifications we have
SELECT 'Current Qualifications:' as Info;
SELECT Id, Name FROM Qualifications WHERE IsActive = 1;

-- Create a temporary table to store qualification IDs
CREATE TABLE #TempQualifications (Id INT, Name NVARCHAR(100));

-- Insert all active qualifications into temp table
INSERT INTO #TempQualifications (Id, Name)
SELECT Id, Name FROM Qualifications WHERE IsActive = 1;

-- Declare variables
DECLARE @QualId INT, @QualName NVARCHAR(100), @Counter INT = 1;
DECLARE @InternationalExams TABLE (Name NVARCHAR(200), Description NVARCHAR(500));

-- Insert international exam names
INSERT INTO @InternationalExams (Name, Description) VALUES
('SAT', 'International SAT exam'),
('IELTS', 'International IELTS exam'),
('TOEFL', 'International TOEFL exam'),
('GRE', 'International GRE exam'),
('GMAT', 'International GMAT exam'),
('PTE', 'International PTE exam'),
('OET', 'International OET exam'),
('Duolingo English Test', 'International Duolingo English Test'),
('Cambridge English', 'International Cambridge English exam'),
('LSAT', 'International LSAT exam'),
('MCAT', 'International MCAT exam'),
('ACT', 'International ACT exam'),
('AP Exams', 'International AP Exams'),
('IB Diploma', 'International IB Diploma exam'),
('FCE', 'International FCE exam'),
('CAE', 'International CAE exam');

-- Create a cursor to iterate through qualifications
DECLARE QualCursor CURSOR FOR
SELECT Id, Name FROM #TempQualifications;

OPEN QualCursor;
FETCH NEXT FROM QualCursor INTO @QualId, @QualName;

-- Create a temporary table for numbered international exams
CREATE TABLE #NumberedExams (RowNum INT, Name NVARCHAR(200), Description NVARCHAR(500));
INSERT INTO #NumberedExams (RowNum, Name, Description)
SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)), Name, Description 
FROM @InternationalExams;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Check if international exam already exists for this qualification
    IF NOT EXISTS (
        SELECT 1 FROM Exams e
        JOIN ExamQualifications eq ON e.Id = eq.ExamId
        WHERE eq.QualificationId = @QualId AND e.IsInternational = 1
    )
    BEGIN
        -- Get an international exam name based on counter
        DECLARE @ExamName NVARCHAR(200), @ExamDesc NVARCHAR(500);
        
        -- If counter exceeds available exams, cycle back
        IF @Counter > 16
            SET @Counter = 1;
            
        -- Get the specific exam for this counter
        SELECT @ExamName = Name, @ExamDesc = Description
        FROM #NumberedExams
        WHERE RowNum = @Counter;
        
        -- Insert the international exam
        INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
        VALUES (
            @ExamName + ' - ' + @QualName,
            @ExamDesc + ' for ' + @QualName + ' qualification',
            180, -- 3 hours
            100,
            60, -- Higher passing marks for international exams
            NULL,
            1, -- IsInternational = true
            1, -- IsActive = true
            GETDATE()
        );
        
        -- Get the newly inserted exam ID
        DECLARE @NewExamId INT = SCOPE_IDENTITY();
        
        -- Create the relationship
        INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
        VALUES (@NewExamId, @QualId, GETDATE(), 1);
        
        PRINT 'Created international exam: ' + @ExamName + ' - ' + @QualName;
        
        SET @Counter = @Counter + 1;
    END
    ELSE
    BEGIN
        PRINT 'International exam already exists for qualification: ' + @QualName;
    END
    
    FETCH NEXT FROM QualCursor INTO @QualId, @QualName;
END

CLOSE QualCursor;
DEALLOCATE QualCursor;

-- Clean up
DROP TABLE #TempQualifications;
DROP TABLE #NumberedExams;

-- Verify the results
SELECT 'International Exams Created:' as Info;
SELECT 
    e.Id,
    e.Name,
    e.IsInternational,
    e.DurationInMinutes,
    e.TotalMarks,
    e.PassingMarks,
    q.Name as QualificationName,
    e.CreatedAt
FROM Exams e
JOIN ExamQualifications eq ON e.Id = eq.ExamId
JOIN Qualifications q ON eq.QualificationId = q.Id
WHERE e.IsInternational = 1
ORDER BY q.Name, e.Name;
