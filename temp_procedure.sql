Changed database context to 'RankUp_QuestionDB'.
                                                                                                                                                                                                                                                                
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Question_AdminCreate]
    @ModuleId INT,
    @ExamId INT,
    @SubjectId INT,
    @TopicId INT = NULL,
    @Marks INT,
    @NegativeMarks DECIMAL(10,2),
    @Difficulty INT,
    @CorrectAnswer NVARCHAR(1),
    @SameExplanationForAll

(1 rows affected)
