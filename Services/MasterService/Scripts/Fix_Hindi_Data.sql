USE [RankUp_MasterDB];
GO

-- Fix Hindi data with proper UTF-8 encoding
-- Delete existing incorrect Hindi entries
DELETE FROM dbo.ExamLanguages WHERE ExamId = 11 AND LanguageId = 49;
DELETE FROM dbo.ExamLanguages WHERE ExamId = 15 AND LanguageId = 49;

-- Insert correct Hindi data
-- NIMCET – MCA (Hindi)
INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
VALUES (11, 49, N'NIMCET – एमसीए', N'NIMCET – कंप्यूटर एप्लिकेशन्स में मास्टर्स', 1, GETUTCDATE(), GETUTCDATE());

-- Test Exam 1538849947 (Hindi)
INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
VALUES (15, 49, N'टेस्ट परीक्षा 1538849947', N'प्रदर्शन उद्देश्यों के लिए टेस्ट परीक्षा', 1, GETUTCDATE(), GETUTCDATE());

-- Verify the data
SELECT e.Id, e.Name as BaseName, el.LanguageId, l.Code as LanguageCode, el.Name as LocalizedName
FROM dbo.Exams e 
LEFT JOIN dbo.ExamLanguages el ON e.Id = el.ExamId 
LEFT JOIN dbo.Languages l ON el.LanguageId = l.Id 
WHERE e.Id IN (11, 15) 
ORDER BY e.Id, el.LanguageId;

PRINT 'Hindi data fixed successfully!';
