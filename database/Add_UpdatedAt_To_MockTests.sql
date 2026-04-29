-- Add UpdatedAt column to MockTests table
ALTER TABLE MockTests 
ADD UpdatedAt datetime2 NULL;

-- Update existing records to have UpdatedAt = CreatedAt
UPDATE MockTests 
SET UpdatedAt = CreatedAt 
WHERE UpdatedAt IS NULL;

-- Make the column NOT NULL with default value
ALTER TABLE MockTests 
ALTER COLUMN UpdatedAt datetime2 NOT NULL;

-- Create a default constraint for future inserts
ALTER TABLE MockTests 
ADD CONSTRAINT DF_MockTests_UpdatedAt DEFAULT GETUTCDATE() FOR UpdatedAt;
