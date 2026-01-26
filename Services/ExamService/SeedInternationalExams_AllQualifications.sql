-- Seed International Exams for ALL qualifications
-- This script adds international exams (IsInternational = 1) for each qualification
-- Based on the qualification IDs found in the exam data: 4, 5, 6, 7

-- First, let's see what qualifications we have
SELECT 'Current Qualifications:' as Info;
SELECT Id, Name FROM Qualifications WHERE IsActive = 1;

-- Add international exams for each qualification level
-- These exams will have IsInternational = 1 and appropriate passing marks

-- For Qualification ID 4 (e.g., 10th Grade/High School)
INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
VALUES 
('SAT', 'International SAT exam for high school students', 180, 1600, 1000, '/images/exams/sat-highschool.jpg', 1, 1, GETDATE()),
('IELTS Academic', 'International IELTS Academic exam for high school students', 165, 9, 6.5, '/images/exams/ielts-academic.jpg', 1, 1, GETDATE()),
('TOEFL', 'International TOEFL exam for high school students', 180, 120, 80, '/images/exams/toefl-highschool.jpg', 1, 1, GETDATE()),
('Duolingo English Test', 'International Duolingo English Test for high school students', 60, 160, 100, '/images/exams/duolingo-highschool.jpg', 1, 1, GETDATE());

-- For Qualification ID 5 (e.g., 12th Grade/Intermediate)
INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
VALUES 
('SAT', 'International SAT exam for intermediate qualification', 180, 1600, 1100, '/images/exams/sat-intermediate.jpg', 1, 1, GETDATE()),
('IELTS General', 'International IELTS General Training exam', 165, 9, 7, '/images/exams/ielts-general.jpg', 1, 1, GETDATE()),
('TOEFL', 'International TOEFL exam for intermediate qualification', 180, 120, 85, '/images/exams/toefl-intermediate.jpg', 1, 1, GETDATE()),
('PTE Academic', 'International PTE Academic exam', 180, 90, 65, '/images/exams/pte-academic.jpg', 1, 1, GETDATE()),
('ACT', 'International ACT exam for intermediate qualification', 175, 36, 24, '/images/exams/act-intermediate.jpg', 1, 1, GETDATE());

-- For Qualification ID 6 (e.g., Graduation)
INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
VALUES 
('GRE', 'International GRE exam for graduate studies', 230, 340, 300, '/images/exams/gre-graduation.jpg', 1, 1, GETDATE()),
('GMAT', 'International GMAT exam for MBA programs', 187, 800, 600, '/images/exams/gmat-graduation.jpg', 1, 1, GETDATE()),
('IELTS Academic', 'International IELTS Academic exam for graduates', 165, 9, 7.5, '/images/exams/ielts-academic-grad.jpg', 1, 1, GETDATE()),
('TOEFL', 'International TOEFL exam for graduates', 180, 120, 90, '/images/exams/toefl-graduation.jpg', 1, 1, GETDATE()),
('PTE Academic', 'International PTE Academic exam for graduates', 180, 90, 70, '/images/exams/pte-academic-grad.jpg', 1, 1, GETDATE()),
('Cambridge English', 'International Cambridge English exam for graduates', 240, 210, 180, '/images/exams/cambridge-english.jpg', 1, 1, GETDATE());

-- For Qualification ID 7 (e.g., Post Graduation/PhD)
INSERT INTO Exams (Name, Description, DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational, IsActive, CreatedAt)
VALUES 
('LSAT', 'International LSAT exam for law schools', 155, 180, 150, '/images/exams/lsat-postgrad.jpg', 1, 1, GETDATE()),
('MCAT', 'International MCAT exam for medical schools', 475, 528, 500, '/images/exams/mcat-postgrad.jpg', 1, 1, GETDATE()),
('GRE Advanced', 'International GRE Advanced exam for postgraduate studies', 230, 340, 320, '/images/exams/gre-advanced.jpg', 1, 1, GETDATE()),
('GMAT Advanced', 'International GMAT Advanced exam for advanced MBA programs', 187, 800, 700, '/images/exams/gmat-advanced.jpg', 1, 1, GETDATE()),
('OET', 'International OET exam for healthcare professionals', 180, 500, 400, '/images/exams/oet-postgrad.jpg', 1, 1, GETDATE()),
('IB Diploma', 'International IB Diploma program for advanced studies', 240, 45, 35, '/images/exams/ib-diploma.jpg', 1, 1, GETDATE()),
('FCE', 'International First Certificate in English', 240, 230, 200, '/images/exams/fce-postgrad.jpg', 1, 1, GETDATE()),
('CAE', 'International Certificate in Advanced English', 240, 210, 190, '/images/exams/cae-postgrad.jpg', 1, 1, GETDATE());

-- Now add the relationships between exams and qualifications
-- Get the exam IDs that were just inserted (they will be the highest IDs)

-- For Qualification ID 4
INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
SELECT Id, 4, GETDATE(), 1 FROM Exams 
WHERE Name IN ('SAT', 'IELTS Academic', 'TOEFL', 'Duolingo English Test') 
AND Description LIKE '%high school students%' AND IsInternational = 1;

-- For Qualification ID 5
INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
SELECT Id, 5, GETDATE(), 1 FROM Exams 
WHERE Name IN ('SAT', 'IELTS General', 'TOEFL', 'PTE Academic', 'ACT') 
AND Description LIKE '%intermediate qualification%' AND IsInternational = 1;

-- For Qualification ID 6
INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
SELECT Id, 6, GETDATE(), 1 FROM Exams 
WHERE Name IN ('GRE', 'GMAT', 'IELTS Academic', 'TOEFL', 'PTE Academic', 'Cambridge English') 
AND (Description LIKE '%graduate studies%' OR Description LIKE '%graduates%') AND IsInternational = 1;

-- For Qualification ID 7
INSERT INTO ExamQualifications (ExamId, QualificationId, CreatedAt, IsActive)
SELECT Id, 7, GETDATE(), 1 FROM Exams 
WHERE Name IN ('LSAT', 'MCAT', 'GRE Advanced', 'GMAT Advanced', 'OET', 'IB Diploma', 'FCE', 'CAE') 
AND Description LIKE '%postgraduate%' OR Description LIKE '%advanced%' OR Description LIKE '%law schools%' OR Description LIKE '%medical schools%' OR Description LIKE '%healthcare professionals%' 
AND IsInternational = 1;

-- Verify the international exams were added
SELECT 'International Exams Created:' as Info;
SELECT 
    e.Id,
    e.Name,
    e.IsInternational,
    e.DurationInMinutes,
    e.TotalMarks,
    e.PassingMarks,
    q.Name as QualificationName,
    q.Id as QualificationId,
    e.CreatedAt
FROM Exams e
JOIN ExamQualifications eq ON e.Id = eq.ExamId
JOIN Qualifications q ON eq.QualificationId = q.Id
WHERE e.IsInternational = 1
ORDER BY q.Id, e.Name;

-- Summary by qualification
SELECT 'Summary by Qualification:' as Info;
SELECT 
    q.Name as QualificationName,
    q.Id as QualificationId,
    COUNT(e.Id) as InternationalExamCount
FROM Qualifications q
LEFT JOIN ExamQualifications eq ON q.Id = eq.QualificationId
LEFT JOIN Exams e ON eq.ExamId = e.Id AND e.IsInternational = 1
WHERE q.IsActive = 1
GROUP BY q.Id, q.Name
ORDER BY q.Id;
