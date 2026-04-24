-- =============================================
-- Add Enhanced Fields to MockTests Table
-- =============================================

-- Add new fields to MockTests table
ALTER TABLE [dbo].[MockTests]
ADD 
    [MockTestType] INT NOT NULL DEFAULT 1, -- 1=MockTest, 2=TestSeries, 3=DeepPractice, 4=PreviousYear
    [SubjectId] INT NULL, -- For specific subject tests
    [TopicId] INT NULL, -- For topic-wise tests (DeepPractice)
    [HasNegativeMarking] BIT NOT NULL DEFAULT 0,
    [NegativeMarkingValue] DECIMAL(5,2) NULL,
    [MarksPerQuestion] DECIMAL(5,2) NOT NULL DEFAULT 5.00,
    [ExamDate] DATE NULL, -- For scheduled exams
    [PublishDateTime] DATETIME2 NULL, -- When to publish the test
    [ValidTill] DATETIME2 NULL, -- When test expires
    [ShowResultType] NVARCHAR(20) NOT NULL DEFAULT 'Immediate', -- Immediate, Scheduled, AfterDeadline
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Active', -- Active, Inactive, Draft
    [Year] INT NULL, -- For PreviousYear papers
    [Difficulty] NVARCHAR(20) NULL, -- Easy, Medium, Hard (for DeepPractice)
    [PaperCode] NVARCHAR(50) NULL, -- For TestSeries
    [ImageUrl] NVARCHAR(500) NULL; -- For uploaded images

-- Add foreign key constraints for new fields
ALTER TABLE [dbo].[MockTests]
ADD CONSTRAINT [FK_MockTests_Subjects] FOREIGN KEY ([SubjectId]) REFERENCES [dbo].[Subjects]([Id]);

ALTER TABLE [dbo].[MockTests]
ADD CONSTRAINT [FK_MockTests_Topics] FOREIGN KEY ([TopicId]) REFERENCES [dbo].[Topics]([Id]);

-- Add check constraints
ALTER TABLE [dbo].[MockTests]
ADD CONSTRAINT [CK_MockTests_MockTestType] CHECK ([MockTestType] IN (1, 2, 3, 4));

ALTER TABLE [dbo].[MockTests]
ADD CONSTRAINT [CK_MockTests_ShowResultType] CHECK ([ShowResultType] IN ('Immediate', 'Scheduled', 'AfterDeadline'));

ALTER TABLE [dbo].[MockTests]
ADD CONSTRAINT [CK_MockTests_Status] CHECK ([Status] IN ('Active', 'Inactive', 'Draft'));

ALTER TABLE [dbo].[MockTests]
ADD CONSTRAINT [CK_MockTests_Difficulty] CHECK ([Difficulty] IN ('Easy', 'Medium', 'Hard') OR [Difficulty] IS NULL);

ALTER TABLE [dbo].[MockTests]
ADD CONSTRAINT [CK_MockTests_NegativeMarkingValue] CHECK (
    ([HasNegativeMarking] = 0 AND [NegativeMarkingValue] IS NULL) OR 
    ([HasNegativeMarking] = 1 AND [NegativeMarkingValue] IS NOT NULL AND [NegativeMarkingValue] >= 0)
);

-- Add indexes for new fields
CREATE INDEX [IX_MockTests_MockTestType] ON [dbo].[MockTests]([MockTestType]);
CREATE INDEX [IX_MockTests_SubjectId] ON [dbo].[MockTests]([SubjectId]);
CREATE INDEX [IX_MockTests_TopicId] ON [dbo].[MockTests]([TopicId]);
CREATE INDEX [IX_MockTests_Status] ON [dbo].[MockTests]([Status]);
CREATE INDEX [IX_MockTests_ExamDate] ON [dbo].[MockTests]([ExamDate]);
CREATE INDEX [IX_MockTests_PublishDateTime] ON [dbo].[MockTests]([PublishDateTime]);
CREATE INDEX [IX_MockTests_ValidTill] ON [dbo].[MockTests]([ValidTill]);

-- Update existing records to have proper default values
UPDATE [dbo].[MockTests] 
SET [Status] = 'Active' 
WHERE [IsActive] = 1;

UPDATE [dbo].[MockTests] 
SET [Status] = 'Inactive' 
WHERE [IsActive] = 0;

PRINT 'Enhanced MockTest fields added successfully!';
