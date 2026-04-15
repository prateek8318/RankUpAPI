using Dapper;
using Microsoft.Data.SqlClient;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces;
using QuestionService.Application.DTOs;

namespace QuestionService.Infrastructure.Repositories
{
    public class QuestionRepository : BaseDapperRepository, IQuestionRepository
    {
        public QuestionRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT q.*, t.*, qt.*
                    FROM Questions q
                    LEFT JOIN Topics t ON q.TopicId = t.Id
                    LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
                    WHERE q.Id = @Id AND q.IsActive = 1
                    ORDER BY qt.LanguageCode";

                var questionDict = new Dictionary<int, Question>();

                await connection.QueryAsync<Question, Topic, QuestionTranslation, Question>(
                    sql,
                    (question, topic, translation) =>
                    {
                        if (!questionDict.TryGetValue(question.Id, out var questionEntry))
                        {
                            questionEntry = question;
                            questionEntry.Topic = topic;
                            questionDict.Add(question.Id, questionEntry);
                        }

                        if (translation != null)
                        {
                            questionEntry.Translations.Add(translation);
                        }

                        return question;
                    },
                    new { Id = id },
                    splitOn: "Id,Id"
                );

                return questionDict.Values.FirstOrDefault();
            });
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT q.*, t.*, qt.*
                    FROM Questions q
                    LEFT JOIN Topics t ON q.TopicId = t.Id
                    LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
                    WHERE q.IsActive = 1
                    ORDER BY q.CreatedAt DESC";

                var questionDict = new Dictionary<int, Question>();

                await connection.QueryAsync<Question, Topic, QuestionTranslation, Question>(
                    sql,
                    (question, topic, translation) =>
                    {
                        if (!questionDict.TryGetValue(question.Id, out var questionEntry))
                        {
                            questionEntry = question;
                            questionEntry.Topic = topic;
                            questionDict.Add(question.Id, questionEntry);
                        }

                        if (translation != null)
                        {
                            questionEntry.Translations.Add(translation);
                        }

                        return question;
                    },
                    splitOn: "Id,Id"
                );

                return questionDict.Values;
            });
        }

        public async Task<IEnumerable<Question>> FindAsync(System.Linq.Expressions.Expression<Func<Question, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task<Question> AddAsync(Question entity)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    INSERT INTO Questions (
                        ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD,
                        CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType,
                        QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl,
                        ExplanationImageUrl, SameExplanationForAllLanguages, Reference, Tags, CreatedBy, CreatedAt, IsActive
                    )
                    OUTPUT INSERTED.Id, INSERTED.CreatedAt
                    VALUES (
                        @ModuleId, @ExamId, @SubjectId, @TopicId, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD,
                        @CorrectAnswer, @Explanation, @Marks, @NegativeMarks, @DifficultyLevel, @QuestionType,
                        @QuestionImageUrl, @OptionAImageUrl, @OptionBImageUrl, @OptionCImageUrl, @OptionDImageUrl,
                        @ExplanationImageUrl, @SameExplanationForAllLanguages, @Reference, @Tags, @CreatedBy, GETDATE(), 1
                    )";

                var result = await connection.QuerySingleAsync<(int Id, DateTime CreatedAt)>(sql, entity);
                entity.Id = result.Id;
                entity.CreatedAt = result.CreatedAt;
                entity.IsActive = true;

                return entity;
            });
        }

        public async Task<Question> UpdateAsync(Question entity)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    UPDATE Questions SET
                        ModuleId = @ModuleId,
                        ExamId = @ExamId,
                        SubjectId = @SubjectId,
                        TopicId = @TopicId,
                        QuestionText = @QuestionText,
                        OptionA = @OptionA,
                        OptionB = @OptionB,
                        OptionC = @OptionC,
                        OptionD = @OptionD,
                        CorrectAnswer = @CorrectAnswer,
                        Explanation = @Explanation,
                        Marks = @Marks,
                        NegativeMarks = @NegativeMarks,
                        DifficultyLevel = @DifficultyLevel,
                        QuestionType = @QuestionType,
                        QuestionImageUrl = @QuestionImageUrl,
                        OptionAImageUrl = @OptionAImageUrl,
                        OptionBImageUrl = @OptionBImageUrl,
                        OptionCImageUrl = @OptionCImageUrl,
                        OptionDImageUrl = @OptionDImageUrl,
                        ExplanationImageUrl = @ExplanationImageUrl,
                        SameExplanationForAllLanguages = @SameExplanationForAllLanguages,
                        Reference = @Reference,
                        Tags = @Tags,
                        UpdatedAt = GETDATE()
                    WHERE Id = @Id AND IsActive = 1";

                await connection.ExecuteAsync(sql, entity);
                entity.UpdatedAt = DateTime.UtcNow;
                return entity;
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "UPDATE Questions SET IsActive = 0, UpdatedAt = GETDATE() WHERE Id = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
                return rowsAffected > 0;
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper doesn't track changes, so this is handled by individual operations
            return 0;
        }

        public async Task<Question?> GetByTransactionIdAsync(string transactionId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT q.*, t.*, qt.*
                    FROM Questions q
                    LEFT JOIN Topics t ON q.TopicId = t.Id
                    LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
                    WHERE q.IsActive = 1 AND (q.Tags LIKE @TransactionId OR q.Reference = @TransactionId)";

                var questionDict = new Dictionary<int, Question>();

                await connection.QueryAsync<Question, Topic, QuestionTranslation, Question>(
                    sql,
                    (question, topic, translation) =>
                    {
                        if (!questionDict.TryGetValue(question.Id, out var questionEntry))
                        {
                            questionEntry = question;
                            questionEntry.Topic = topic;
                            questionDict.Add(question.Id, questionEntry);
                        }

                        if (translation != null)
                        {
                            questionEntry.Translations.Add(translation);
                        }

                        return question;
                    },
                    new { TransactionId = $"%{transactionId}%" },
                    splitOn: "Id,Id"
                );

                return questionDict.Values.FirstOrDefault();
            });
        }

        public async Task<(IEnumerable<Question> Questions, int TotalCount)> GetQuestionsPagedAsync(int pageNumber, int pageSize, int? examId = null, int? subjectId = null, int? topicId = null, string? difficultyLevel = null, bool? isPublished = null, string languageCode = "en")
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[GetQuestionsWithFilters] @ExamId, @SubjectId, @TopicId, @DifficultyLevel, @IsPublished, @LanguageCode, @PageNumber, @PageSize";

                var parameters = new
                {
                    ExamId = examId,
                    SubjectId = subjectId,
                    TopicId = topicId,
                    DifficultyLevel = difficultyLevel,
                    IsPublished = isPublished,
                    LanguageCode = languageCode,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                using var multi = await connection.QueryMultipleAsync(sql, parameters);

                var questionLookup = new Dictionary<int, Question>();
                var questions = multi.Read<Question, Topic, QuestionTranslation, Question>(
                    (question, topic, translation) =>
                    {
                        if (!questionLookup.TryGetValue(question.Id, out var existingQuestion))
                        {
                            existingQuestion = question;
                            existingQuestion.Topic = topic;
                            questionLookup[existingQuestion.Id] = existingQuestion;
                        }

                        if (translation != null)
                        {
                            existingQuestion.Translations.Add(translation);
                        }

                        return existingQuestion;
                    },
                    splitOn: "Id,Id"
                );

                var totalCount = await multi.ReadSingleAsync<int>();

                return (questionLookup.Values, totalCount);
            });
        }

        public async Task<(int TotalQuestions, int AddedToday, int NegativeMarksCount, int UnpublishedCount)> GetStatisticsAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[GetQuestionStatistics]";
                
                using var multi = await connection.QueryMultipleAsync(sql);
                
                var overallStats = await multi.ReadSingleAsync<(int TotalQuestions, int AddedToday, int NegativeMarksCount, int UnpublishedCount, int EasyCount, int MediumCount, int HardCount)>();
                var subjectStats = await multi.ReadAsync<SubjectQuestionCountDto>();
                var examStats = await multi.ReadAsync<ExamQuestionCountDto>();

                return (overallStats.TotalQuestions, overallStats.AddedToday, overallStats.NegativeMarksCount, overallStats.UnpublishedCount);
            });
        }

        public async Task<Question> CreateQuestionAsync(string questionText, string? optionA, string? optionB, string? optionC, string? optionD, string correctAnswer, decimal marks, int examId, int subjectId, int? topicId = null, string difficultyLevel = "Medium", int createdBy = 0)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[CreateQuestionWithTranslation] @ModuleId, @ExamId, @SubjectId, @TopicId, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD, @CorrectAnswer, @Explanation, @Marks, @NegativeMarks, @DifficultyLevel, @QuestionType, @QuestionImageUrl, @OptionAImageUrl, @OptionBImageUrl, @OptionCImageUrl, @OptionDImageUrl, @ExplanationImageUrl, @SameExplanationForAllLanguages, @Reference, @Tags, @CreatedBy, @LanguageCode, @TranslatedQuestionText, @TranslatedOptionA, @TranslatedOptionB, @TranslatedOptionC, @TranslatedOptionD, @TranslatedExplanation";

                var parameters = new
                {
                    ModuleId = 0, // Not provided in new signature
                    ExamId = examId,
                    SubjectId = subjectId,
                    TopicId = topicId,
                    QuestionText = questionText,
                    OptionA = optionA ?? "",
                    OptionB = optionB ?? "",
                    OptionC = optionC ?? "",
                    OptionD = optionD ?? "",
                    CorrectAnswer = correctAnswer,
                    Explanation = "", // Not provided in new signature
                    Marks = marks,
                    NegativeMarks = 0, // Not provided in new signature
                    DifficultyLevel = difficultyLevel,
                    QuestionType = "MCQ", // Not provided in new signature
                    QuestionImageUrl = "", // Not provided in new signature
                    OptionAImageUrl = "", // Not provided in new signature
                    OptionBImageUrl = "", // Not provided in new signature
                    OptionCImageUrl = "", // Not provided in new signature
                    OptionDImageUrl = "", // Not provided in new signature
                    ExplanationImageUrl = "", // Not provided in new signature
                    SameExplanationForAllLanguages = false, // Not provided in new signature
                    Reference = "", // Not provided in new signature
                    Tags = "", // Not provided in new signature
                    CreatedBy = createdBy,
                    LanguageCode = "en", // Default language since translations not provided in new signature
                    TranslatedQuestionText = questionText,
                    TranslatedOptionA = optionA ?? "",
                    TranslatedOptionB = optionB ?? "",
                    TranslatedOptionC = optionC ?? "",
                    TranslatedOptionD = optionD ?? "",
                    TranslatedExplanation = ""
                };

                var questionId = await connection.QuerySingleAsync<int>(sql, parameters);
                
                // Return the created question
                return await GetByIdAsync(questionId);
            });
        }

        public async Task<bool> UpdateQuestionAsync(int id, string? questionText, string? optionA, string? optionB, string? optionC, string? optionD, string? correctAnswer, decimal? marks, string? difficultyLevel)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    UPDATE Questions SET
                        ModuleId = @ModuleId,
                        TopicId = @TopicId,
                        QuestionText = @QuestionText,
                        OptionA = @OptionA,
                        OptionB = @OptionB,
                        OptionC = @OptionC,
                        OptionD = @OptionD,
                        CorrectAnswer = @CorrectAnswer,
                        Explanation = @Explanation,
                        Marks = @Marks,
                        NegativeMarks = @NegativeMarks,
                        DifficultyLevel = @DifficultyLevel,
                        QuestionType = @QuestionType,
                        QuestionImageUrl = @QuestionImageUrl,
                        OptionAImageUrl = @OptionAImageUrl,
                        OptionBImageUrl = @OptionBImageUrl,
                        OptionCImageUrl = @OptionCImageUrl,
                        OptionDImageUrl = @OptionDImageUrl,
                        ExplanationImageUrl = @ExplanationImageUrl,
                        SameExplanationForAllLanguages = @SameExplanationForAllLanguages,
                        Reference = @Reference,
                        Tags = @Tags,
                        UpdatedAt = GETDATE()
                    WHERE Id = @Id AND IsActive = 1";

                var parameters = new
                {
                    Id = id,
                    ModuleId = 0, // Not provided in new signature
                    TopicId = 0, // Not provided in new signature
                    QuestionText = questionText,
                    OptionA = optionA,
                    OptionB = optionB,
                    OptionC = optionC,
                    OptionD = optionD,
                    CorrectAnswer = correctAnswer,
                    Explanation = "", // Not provided in new signature
                    Marks = marks,
                    NegativeMarks = 0, // Not provided in new signature
                    DifficultyLevel = difficultyLevel,
                    QuestionType = "MCQ", // Not provided in new signature
                    QuestionImageUrl = "", // Not provided in new signature
                    OptionAImageUrl = "", // Not provided in new signature
                    OptionBImageUrl = "", // Not provided in new signature
                    OptionCImageUrl = "", // Not provided in new signature
                    OptionDImageUrl = "", // Not provided in new signature
                    ExplanationImageUrl = "", // Not provided in new signature
                    SameExplanationForAllLanguages = false, // Not provided in new signature
                    Reference = "", // Not provided in new signature
                    Tags = "" // Not provided in new signature
                };

                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
                return rowsAffected > 0;
            });
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "UPDATE Questions SET IsActive = 0, UpdatedAt = GETDATE() WHERE Id = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
                return rowsAffected > 0;
            });
        }

        public async Task<bool> TogglePublishStatusAsync(int questionId, bool isPublished, int reviewedBy)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[ToggleQuestionPublishStatus] @QuestionId, @IsPublished, @ReviewedBy";

                var parameters = new
                {
                    QuestionId = questionId,
                    IsPublished = isPublished,
                    ReviewedBy = reviewedBy
                };

                var rowsAffected = await connection.QuerySingleAsync<int>(sql, parameters);
                return rowsAffected > 0;
            });
        }

        public async Task<IEnumerable<Question>> GetByExamIdAsync(int examId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT q.*, t.*, qt.*
                    FROM Questions q
                    LEFT JOIN Topics t ON q.TopicId = t.Id
                    LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
                    WHERE q.ExamId = @ExamId AND q.IsActive = 1
                    ORDER BY q.CreatedAt DESC";

                var questionDict = new Dictionary<int, Question>();

                await connection.QueryAsync<Question, Topic, QuestionTranslation, Question>(
                    sql,
                    (question, topic, translation) =>
                    {
                        if (!questionDict.TryGetValue(question.Id, out var questionEntry))
                        {
                            questionEntry = question;
                            questionEntry.Topic = topic;
                            questionDict.Add(question.Id, questionEntry);
                        }

                        if (translation != null)
                        {
                            questionEntry.Translations.Add(translation);
                        }

                        return question;
                    },
                    new { ExamId = examId },
                    splitOn: "Id,Id"
                );

                return questionDict.Values;
            });
        }

        public async Task<IEnumerable<Question>> GetBySubjectIdAsync(int subjectId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT q.*, t.*, qt.*
                    FROM Questions q
                    LEFT JOIN Topics t ON q.TopicId = t.Id
                    LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
                    WHERE q.SubjectId = @SubjectId AND q.IsActive = 1
                    ORDER BY q.CreatedAt DESC";

                var questionDict = new Dictionary<int, Question>();

                await connection.QueryAsync<Question, Topic, QuestionTranslation, Question>(
                    sql,
                    (question, topic, translation) =>
                    {
                        if (!questionDict.TryGetValue(question.Id, out var questionEntry))
                        {
                            questionEntry = question;
                            questionEntry.Topic = topic;
                            questionDict.Add(question.Id, questionEntry);
                        }

                        if (translation != null)
                        {
                            questionEntry.Translations.Add(translation);
                        }

                        return question;
                    },
                    new { SubjectId = subjectId },
                    splitOn: "Id,Id"
                );

                return questionDict.Values;
            });
        }

        public async Task<IEnumerable<Question>> GetByTopicIdAsync(int topicId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT q.*, t.*, qt.*
                    FROM Questions q
                    LEFT JOIN Topics t ON q.TopicId = t.Id
                    LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
                    WHERE q.TopicId = @TopicId AND q.IsActive = 1
                    ORDER BY q.CreatedAt DESC";

                var questionDict = new Dictionary<int, Question>();

                await connection.QueryAsync<Question, Topic, QuestionTranslation, Question>(
                    sql,
                    (question, topic, translation) =>
                    {
                        if (!questionDict.TryGetValue(question.Id, out var questionEntry))
                        {
                            questionEntry = question;
                            questionEntry.Topic = topic;
                            questionDict.Add(question.Id, questionEntry);
                        }

                        if (translation != null)
                        {
                            questionEntry.Translations.Add(translation);
                        }

                        return question;
                    },
                    new { TopicId = topicId },
                    splitOn: "Id,Id"
                );

                return questionDict.Values;
            });
        }
    }
}
