-- Script to populate ExamQualifications table for existing exams
-- This will add qualification and stream mappings for exams that don't have them

-- First, let's check which exams don't have qualifications
SELECT 
    e.Id,
    e.Name,
    e.IsInternational,
    CASE 
        WHEN eq.ExamId IS NULL THEN 'Missing Qualification'
        ELSE 'Has Qualification'
    END AS Status
FROM Exams e
LEFT JOIN ExamQualifications eq ON e.Id = eq.ExamId AND eq.IsActive = 1
WHERE e.IsActive = 1
ORDER BY e.Id;

-- Insert qualifications for existing exams that don't have them
-- For Indian exams (IsInternational = 0), assign common qualifications
-- For International exams, assign international qualifications

INSERT INTO ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt, UpdatedAt)
SELECT 
    e.Id as ExamId,
    -- Assign qualification based on exam type
    CASE 
        WHEN e.IsInternational = 0 THEN 
            CASE 
                WHEN e.Id % 3 = 0 THEN 1  -- 10th
                WHEN e.Id % 3 = 1 THEN 2  -- 12th
                ELSE 3                     -- Graduate
            END
        ELSE 
            CASE 
                WHEN e.Id % 2 = 0 THEN 4  -- Bachelor's International
                ELSE 5                     -- Master's International
            END
    END as QualificationId,
    -- Assign stream for Indian exams (null for international)
    CASE 
        WHEN e.IsInternational = 0 THEN 
            CASE 
                WHEN e.Id % 3 = 0 THEN 1  -- Science
                WHEN e.Id % 3 = 1 THEN 2  -- Commerce
                ELSE 3                     -- Arts
            END
        ELSE NULL
    END as StreamId,
    1 as IsActive,
    GETDATE() as CreatedAt,
    GETDATE() as UpdatedAt
FROM Exams e
WHERE e.IsActive = 1
AND NOT EXISTS (
    SELECT 1 FROM ExamQualifications eq 
    WHERE eq.ExamId = e.Id AND eq.IsActive = 1
);

-- Verify the results
SELECT 
    e.Id,
    e.Name,
    e.IsInternational,
    eq.QualificationId,
    eq.StreamId
FROM Exams e
INNER JOIN ExamQualifications eq ON e.Id = eq.ExamId AND eq.IsActive = 1
WHERE e.IsActive = 1
ORDER BY e.Id;
