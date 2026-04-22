-- Complete User Profile for User ID 44
-- This script updates all required fields to mark profile as completed
SET QUOTED_IDENTIFIER ON;
GO

UPDATE [RankUp_UserDB].[dbo].[Users]
SET 
    Name = 'Test User',
    Email = 'testuser44@example.com',
    Gender = 'Male',
    DateOfBirth = '1995-01-15',
    StateId = 65, -- Uttar Pradesh
    LanguageId = 50, -- English
    QualificationId = 1, -- B.Tech
    CategoryId = 2, -- general
    StreamId = 1, -- Computer Science
    ProfilePhoto = '/images/profiles/user44.jpg',
    ExamId = 31, -- RRB
    InterestedInIntlExam = 1,
    CountryCode = '+91',
    UpdatedAt = GETUTCDATE()
WHERE Id = 44;

-- Verify the update
SELECT 
    Id, Name, Email, PhoneNumber, Gender, DateOfBirth, 
    StateId, LanguageId, QualificationId, CategoryId, StreamId,
    ProfilePhoto, ExamId, InterestedInIntlExam, UpdatedAt
FROM [RankUp_UserDB].[dbo].[Users] 
WHERE Id = 44;
