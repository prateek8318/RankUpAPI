-- Update Mock Test ID 12 AccessType from Paid to Free
UPDATE MockTests
SET AccessType = 'Free',
    UpdatedAt = GETDATE()
WHERE Id = 12;

-- Verify the update
SELECT 
    Id,
    Name as Title,
    AccessType,
    UpdatedAt
FROM MockTests
WHERE Id = 12;
