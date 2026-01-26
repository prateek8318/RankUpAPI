-- Seed International Exams for all qualifications
-- This script adds international exams (IsInternational = 1) for each qualification

-- First, let's see what qualifications we have
SELECT Id, Name FROM Qualifications WHERE IsActive = 1;

-- Add international exams for each qualification
-- These exams will have IsInternational = 1 and higher passing marks

-- Example: If you have qualifications with IDs 1, 2, 3, etc.
-- Adjust the qualification IDs based on your actual data

-- For Qualification ID 1 (e.g., 10th Grade)
INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
VALUES 
('SAT - 10th Grade', 'International SAT exam for 10th Grade qualification', 180, 100, 60, NULL, 1, 1, GETDATE()),
('IELTS - 10th Grade', 'International IELTS exam for 10th Grade qualification', 180, 100, 60, NULL, 1, 1, GETDATE());

-- For Qualification ID 2 (e.g., 12th Grade)
INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
VALUES 
('TOEFL - 12th Grade', 'International TOEFL exam for 12th Grade qualification', 180, 100, 60, NULL, 1, 1, GETDATE()),
('GRE - 12th Grade', 'International GRE exam for 12th Grade qualification', 180, 100, 60, NULL, 1, 1, GETDATE());

-- For Qualification ID 3 (e.g., Graduation)
INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
VALUES 
('GMAT - Graduation', 'International GMAT exam for Graduation qualification', 180, 100, 60, NULL, 1, 1, GETDATE()),
('PTE - Graduation', 'International PTE exam for Graduation qualification', 180, 100, 60, NULL, 1, 1, GETDATE());

-- For Qualification ID 4 (e.g., Post Graduation)
INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
VALUES 
('LSAT - Post Graduation', 'International LSAT exam for Post Graduation qualification', 180, 100, 60, NULL, 1, 1, GETDATE()),
('MCAT - Post Graduation', 'International MCAT exam for Post Graduation qualification', 180, 100, 60, NULL, 1, 1, GETDATE());

-- For Qualification ID 5 (e.g., Diploma)
INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
VALUES 
('ACT - Diploma', 'International ACT exam for Diploma qualification', 180, 100, 60, NULL, 1, 1, GETDATE()),
('OET - Diploma', 'International OET exam for Diploma qualification', 180, 100, 60, NULL, 1, 1, GETDATE());

-- For Qualification ID 6 (e.g., PhD)
INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
VALUES 
('AP Exams - PhD', 'International AP Exams for PhD qualification', 180, 100, 60, NULL, 1, 1, GETDATE()),
('IB Diploma - PhD', 'International IB Diploma for PhD qualification', 180, 100, 60, NULL, 1, 1, GETDATE());

-- Now add the relationships between exams and qualifications
-- Get the exam IDs that were just inserted (they will be the highest IDs)

-- For 10th Grade exams (assuming qualification ID = 1)
DECLARE @ExamId1 INT, @ExamId2 INT, @ExamId3 INT, @ExamId4 INT, @ExamId5 INT, @ExamId6 INT, @ExamId7 INT, @ExamId8 INT, @ExamId9 INT, @ExamId10 INT, @ExamId11 INT, @ExamId12 INT;

SELECT TOP 12 @ExamId1 = Id, @ExamId2 = Id, @ExamId3 = Id, @ExamId4 = Id, @ExamId5 = Id, @ExamId6 = Id, @ExamId7 = Id, @ExamId8 = Id, @ExamId9 = Id, @ExamId10 = Id, @ExamId11 = Id, @ExamId12 = Id 
FROM Exams 
WHERE IsInternational = 1 
ORDER BY Id DESC;

-- Insert into ExamQualifications table
-- Adjust qualification IDs based on your actual data

-- For Qualification ID 1 (10th Grade)
INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
VALUES (@ExamId1, 1, GETDATE(), 1);

INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
VALUES (@ExamId2, 1, GETDATE(), 1);

-- For Qualification ID 2 (12th Grade)
INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
VALUES (@ExamId3, 2, GETDATE(), 1);

INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
VALUES (@ExamId4, 2, GETDATE(), 1);

-- For Qualification ID 3 (Graduation)
INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
VALUES (@ExamId5, 3, GETDATE(), 1);

INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
VALUES (@ExamId6, 3, GETDATE(), 1);

-- For Qualification ID 4 (Post Graduation)
INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
VALUES (@ExamId7, 4, GETDATE(), 1);

INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
VALUES (@ExamId8, 4, GETDATE(), 1);

-- For Qualification ID 5 (Diploma)
INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
VALUES (@ExamId9, 5, GETDATE(), 1);

INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
VALUES (@ExamId10, 5, GETDATE(), 1);

-- For Qualification ID 6 (PhD)
INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
VALUES (@ExamId11, 6, GETDATE(), 1);

INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
VALUES (@ExamId12, 6, GETDATE(), 1);

-- Verify the international exams were added
SELECT 
    e.Id,
    e.Name,
    e.IsInternational,
    e.DurationInMinutes,
    e.TotalMarks,
    e.PassingMarks,
    q.Name as QualificationName
FROM Exams e
JOIN ExamQualifications eq ON e.Id = eq.ExamId
JOIN Qualifications q ON eq.QualificationId = q.Id
WHERE e.IsInternational = 1
ORDER BY q.Name, e.Name;
