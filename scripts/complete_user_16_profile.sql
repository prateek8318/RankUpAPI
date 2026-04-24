-- Complete User Profile for User ID 16
-- This script updates all required fields to mark profile as completed
SET QUOTED_IDENTIFIER ON;
GO

UPDATE [RankUp_UserDB].[dbo].[Users]
SET 
    Name = 'TestUser',
    Email = 'riten@gmail.com',
    Gender = 'Male',
    DateOfBirth = '1988-01-19',
    StateId = 65, -- Uttar Pradesh
    LanguageId = 50, -- English
    QualificationId = 4, -- Based on existing data
    CategoryId = 7, -- Based on existing data
    StreamId = 7, -- Based on existing data
    ProfilePhoto = 'uploads/profiles/user_16_20260424110029.jpg',
    ExamId = 31, -- RRB
    InterestedInIntlExam = 1,
    CountryCode = '+91',
    UpdatedAt = GETUTCDATE()
WHERE Id = 16;

-- Verify the update
SELECT 
    Id, Name, Email, PhoneNumber, Gender, DateOfBirth, 
    StateId, LanguageId, QualificationId, CategoryId, StreamId,
    ProfilePhoto, ExamId, InterestedInIntlExam, CountryCode, UpdatedAt
FROM [RankUp_UserDB].[dbo].[Users] 
WHERE Id = 16;
