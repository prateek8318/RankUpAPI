using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using QuestionService.Application.Interfaces;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces;
using QuestionService.Application.DTOs;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace QuestionService.Infrastructure.Repositories
{
    public class QuestionRepository : BaseDapperRepository, QuestionService.Domain.Interfaces.IQuestionRepository, IQuestionFeatureRepository
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

        // Image upload methods
        public async Task<string> UploadQuestionImageAsync(IFormFile image, string imageType, int? questionId, string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                // Generate unique filename
                var fileExtension = Path.GetExtension(image.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadPath = Path.Combine("uploads", "questions", DateTime.Now.ToString("yyyy-MM-dd"));
                var fullPath = Path.Combine(uploadPath, uniqueFileName);

                // Save file (in real implementation, this would save to cloud storage)
                // For now, return a mock URL
                var imageUrl = $"https://rankup-api.s3.amazonaws.com/{fullPath}";

                // Update question with image URL if questionId is provided
                if (questionId.HasValue)
                {
                    var updateSql = imageType.ToLower() switch
                    {
                        "question" => "UPDATE Questions SET QuestionImageUrl = @ImageUrl WHERE Id = @QuestionId",
                        "optiona" => "UPDATE Questions SET OptionAImageUrl = @ImageUrl WHERE Id = @QuestionId",
                        "optionb" => "UPDATE Questions SET OptionBImageUrl = @ImageUrl WHERE Id = @QuestionId",
                        "optionc" => "UPDATE Questions SET OptionCImageUrl = @ImageUrl WHERE Id = @QuestionId",
                        "optiond" => "UPDATE Questions SET OptionDImageUrl = @ImageUrl WHERE Id = @QuestionId",
                        "explanation" => "UPDATE Questions SET ExplanationImageUrl = @ImageUrl WHERE Id = @QuestionId",
                        _ => throw new ArgumentException($"Invalid image type: {imageType}")
                    };

                    await connection.ExecuteAsync(updateSql, new { ImageUrl = imageUrl, QuestionId = questionId.Value });
                }

                return imageUrl;
            });
        }

        // Bulk upload methods
        public async Task<BulkUploadResultDto> BulkCreateQuestionsAsync(BulkQuestionUploadRequestDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[BulkCreateQuestions] @FileName, @ExamId, @SubjectId, @TopicId, @UploadedBy, @LanguageCode, @SkipDuplicates, @ValidateOnly";

                var parameters = new
                {
                    FileName = dto.FileName,
                    ExamId = dto.ExamId,
                    SubjectId = dto.SubjectId,
                    TopicId = dto.TopicId,
                    UploadedBy = dto.UploadedBy,
                    LanguageCode = dto.LanguageCode ?? "en",
                    SkipDuplicates = dto.SkipDuplicates,
                    ValidateOnly = dto.ValidateOnly
                };

                using var multi = await connection.QueryMultipleAsync(sql, parameters);
                var batchId = await multi.ReadSingleAsync<int>();
                var result = await multi.ReadSingleAsync<(int SuccessCount, int FailedCount)>();

                return new BulkUploadResultDto
                {
                    BatchId = batchId,
                    SuccessCount = result.SuccessCount,
                    FailedCount = result.FailedCount
                };
            });
        }

        public async Task<object> GetBulkUploadStatusAsync(int batchId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        b.Id AS BatchId,
                        b.BatchName,
                        b.Status,
                        b.TotalQuestions,
                        b.ProcessedQuestions,
                        b.FailedQuestions,
                        b.ErrorMessage,
                        b.CreatedAt,
                        b.UpdatedAt,
                        e.RowNumber,
                        e.ErrorMessage AS RowErrorMessage
                    FROM QuestionBatches b
                    LEFT JOIN QuestionErrors e ON b.Id = e.BatchId
                    WHERE b.Id = @BatchId AND b.IsActive = 1";

                var batchDict = new Dictionary<int, object>();

                var batches = await connection.QueryAsync(sql, new { BatchId = batchId });

                // For now, return a simple status object
                var batch = batches.FirstOrDefault();
                if (batch == null) return null;

                return new
                {
                    BatchId = batch.BatchId,
                    BatchName = batch.BatchName,
                    Status = batch.Status,
                    TotalQuestions = batch.TotalQuestions,
                    ProcessedQuestions = batch.ProcessedQuestions,
                    FailedQuestions = batch.FailedQuestions,
                    ErrorMessage = batch.ErrorMessage,
                    StartedAt = batch.CreatedAt,
                    CompletedAt = batch.UpdatedAt,
                    SuccessQuestions = Math.Max(0, (int)batch.ProcessedQuestions - (int)batch.FailedQuestions),
                    ErrorMessages = batches
                        .Where(x => x.RowErrorMessage != null)
                        .Select(x => (string)x.RowErrorMessage)
                        .Distinct()
                        .ToList()
                };
            });
        }

        public async Task<List<object>> ParseBulkUploadFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path is required.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Bulk upload file not found.", filePath);
            }

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".csv" => ParseDelimitedFile(filePath, ','),
                ".txt" => ParseDelimitedFile(filePath, ','),
                ".tsv" => ParseDelimitedFile(filePath, '\t'),
                ".xlsx" => ParseExcelFile(filePath),
                _ => throw new NotSupportedException($"Unsupported file format '{extension}'. Supported formats: .csv, .tsv, .txt, .xlsx")
            };
        }

        // Quiz functionality methods
        public async Task<object> StartQuizAsync(QuizStartRequestDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[StartQuizSession] @ExamId, @UserId, @LanguageCode, @SubjectId, @TopicId, @NumberOfQuestions, @DifficultyLevel";

                var parameters = new
                {
                    ExamId = dto.ExamId,
                    UserId = dto.UserId,
                    LanguageCode = dto.LanguageCode ?? "en",
                    SubjectId = dto.SubjectId,
                    TopicId = dto.TopicId,
                    NumberOfQuestions = dto.NumberOfQuestions,
                    DifficultyLevel = dto.DifficultyLevel
                };

                using var multi = await connection.QueryMultipleAsync(sql, parameters);
                var sessionId = await multi.ReadSingleAsync<int>();
                var questions = multi.Read<QuizQuestionDto>().ToList();

                return new
                {
                    Id = sessionId,
                    ExamId = dto.ExamId,
                    UserId = dto.UserId,
                    LanguageCode = dto.LanguageCode ?? "en",
                    StartTime = DateTime.UtcNow,
                    Duration = 60, // Default 60 minutes
                    TotalQuestions = questions.Count,
                    AnsweredQuestions = 0,
                    MarkedForReview = 0,
                    Status = "InProgress",
                    Questions = questions
                };
            });
        }

        public async Task<object> GetQuizSessionAsync(int sessionId, int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[GetQuizSession] @SessionId, @UserId";

                var session = await connection.QuerySingleOrDefaultAsync<object>(sql, new { SessionId = sessionId, UserId = userId });
                return session;
            });
        }

        public async Task<bool> SaveQuizAnswerAsync(QuizAnswerRequestDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SaveQuizAnswer] @QuizSessionId, @QuestionId, @Answer, @MarkForReview, @TimeSpent";

                var parameters = new
                {
                    QuizSessionId = dto.QuizSessionId,
                    QuestionId = dto.QuestionId,
                    Answer = dto.Answer,
                    MarkForReview = dto.MarkForReview,
                    TimeSpent = dto.TimeSpent
                };

                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
                return rowsAffected > 0;
            });
        }

        public async Task<object> SubmitQuizAsync(QuizSubmitRequestDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[SubmitQuiz] @QuizSessionId, @Answers";

                // Convert answers to JSON for stored procedure
                var answersJson = JsonSerializer.Serialize(dto.Answers);

                var parameters = new
                {
                    QuizSessionId = dto.QuizSessionId,
                    Answers = answersJson
                };

                using var multi = await connection.QueryMultipleAsync(sql, parameters);
                var result = await multi.ReadSingleAsync<QuizResultDto>();
                var questionResults = multi.Read<QuestionResultDto>().ToList();

                result.QuestionResults = questionResults;
                return result;
            });
        }

        // Subject and Exam methods
        public async Task<IEnumerable<object>> GetSubjectsAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        s.Id,
                        s.Name,
                        s.Description,
                        COUNT(q.Id) AS QuestionCount,
                        s.IsActive
                    FROM Subjects s
                    LEFT JOIN Questions q ON s.Id = q.SubjectId AND q.IsActive = 1
                    WHERE s.IsActive = 1
                    GROUP BY s.Id, s.Name, s.Description, s.IsActive
                    ORDER BY s.Name";

                return await connection.QueryAsync(sql);
            });
        }

        public async Task<IEnumerable<object>> GetExamsAsync(int? subjectId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        e.Id,
                        e.Name,
                        e.Description,
                        COUNT(q.Id) AS QuestionCount,
                        e.Duration,
                        e.IsActive,
                        0 AS IsLocked -- Mock unlock status
                    FROM Exams e
                    LEFT JOIN Questions q ON e.Id = q.ExamId AND q.IsActive = 1
                    WHERE e.IsActive = 1
                    AND (@SubjectId IS NULL OR e.SubjectId = @SubjectId)
                    GROUP BY e.Id, e.Name, e.Description, e.Duration, e.IsActive
                    ORDER BY e.Name";

                return await connection.QueryAsync(sql, new { SubjectId = subjectId });
            });
        }

        private static List<object> ParseDelimitedFile(string filePath, char delimiter)
        {
            var lines = File.ReadAllLines(filePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            if (lines.Count == 0)
            {
                return new List<object>();
            }

            var headers = SplitDelimitedLine(lines[0], delimiter)
                .Select(NormalizeHeader)
                .ToList();

            var rows = new List<object>();
            for (var index = 1; index < lines.Count; index++)
            {
                var values = SplitDelimitedLine(lines[index], delimiter);
                if (values.All(string.IsNullOrWhiteSpace))
                {
                    continue;
                }

                rows.Add(MapRowToExcelQuestion(index + 1, headers, values));
            }

            return rows;
        }

        private static List<object> ParseExcelFile(string filePath)
        {
            using var archive = ZipFile.OpenRead(filePath);
            var sharedStrings = ReadSharedStrings(archive);
            var worksheetEntry = archive.GetEntry("xl/worksheets/sheet1.xml")
                ?? archive.Entries.FirstOrDefault(x => x.FullName.StartsWith("xl/worksheets/sheet", StringComparison.OrdinalIgnoreCase));

            if (worksheetEntry == null)
            {
                throw new InvalidDataException("The Excel file does not contain any worksheets.");
            }

            using var worksheetStream = worksheetEntry.Open();
            var worksheet = XDocument.Load(worksheetStream);
            XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            var rows = worksheet.Descendants(ns + "sheetData").Descendants(ns + "row").ToList();
            if (rows.Count == 0)
            {
                return new List<object>();
            }

            var headerValues = ReadRowValues(rows[0], sharedStrings, ns)
                .Select(NormalizeHeader)
                .ToList();

            var result = new List<object>();
            foreach (var row in rows.Skip(1))
            {
                var rowNumber = int.TryParse(row.Attribute("r")?.Value, out var parsedRow) ? parsedRow : result.Count + 2;
                var values = ReadRowValues(row, sharedStrings, ns);
                if (values.All(string.IsNullOrWhiteSpace))
                {
                    continue;
                }

                result.Add(MapRowToExcelQuestion(rowNumber, headerValues, values));
            }

            return result;
        }

        private static List<string> ReadSharedStrings(ZipArchive archive)
        {
            var sharedStringsEntry = archive.GetEntry("xl/sharedStrings.xml");
            if (sharedStringsEntry == null)
            {
                return new List<string>();
            }

            using var stream = sharedStringsEntry.Open();
            var document = XDocument.Load(stream);
            XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            return document.Descendants(ns + "si")
                .Select(si => string.Concat(si.Descendants(ns + "t").Select(t => t.Value)))
                .ToList();
        }

        private static List<string> ReadRowValues(XElement row, IReadOnlyList<string> sharedStrings, XNamespace ns)
        {
            var values = new List<string>();
            var cells = row.Elements(ns + "c").ToList();
            var currentColumn = 0;

            foreach (var cell in cells)
            {
                var cellRef = cell.Attribute("r")?.Value;
                var targetColumn = GetColumnIndex(cellRef);
                while (currentColumn < targetColumn)
                {
                    values.Add(string.Empty);
                    currentColumn++;
                }

                values.Add(ReadCellValue(cell, sharedStrings, ns));
                currentColumn++;
            }

            return values;
        }

        private static int GetColumnIndex(string? cellReference)
        {
            if (string.IsNullOrWhiteSpace(cellReference))
            {
                return 0;
            }

            var columnPart = new string(cellReference.TakeWhile(char.IsLetter).ToArray()).ToUpperInvariant();
            var column = 0;
            foreach (var ch in columnPart)
            {
                column = (column * 26) + (ch - 'A' + 1);
            }

            return Math.Max(0, column - 1);
        }

        private static string ReadCellValue(XElement cell, IReadOnlyList<string> sharedStrings, XNamespace ns)
        {
            var type = cell.Attribute("t")?.Value;
            var value = cell.Element(ns + "v")?.Value ?? string.Empty;

            if (type == "s" && int.TryParse(value, out var sharedStringIndex) && sharedStringIndex >= 0 && sharedStringIndex < sharedStrings.Count)
            {
                return sharedStrings[sharedStringIndex];
            }

            if (type == "inlineStr")
            {
                return string.Concat(cell.Descendants(ns + "t").Select(x => x.Value));
            }

            return value;
        }

        private static List<string> SplitDelimitedLine(string line, char delimiter)
        {
            var values = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            for (var i = 0; i < line.Length; i++)
            {
                var ch = line[i];
                if (ch == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }

                    continue;
                }

                if (ch == delimiter && !inQuotes)
                {
                    values.Add(current.ToString().Trim());
                    current.Clear();
                    continue;
                }

                current.Append(ch);
            }

            values.Add(current.ToString().Trim());
            return values;
        }

        private static string NormalizeHeader(string? header)
        {
            return new string((header ?? string.Empty)
                .Trim()
                .Where(char.IsLetterOrDigit)
                .ToArray())
                .ToLowerInvariant();
        }

        private static object MapRowToExcelQuestion(int rowNumber, IReadOnlyList<string> headers, IReadOnlyList<string> values)
        {
            string? Get(params string[] names)
            {
                foreach (var name in names)
                {
                    var index = FindHeaderIndex(headers, name);
                    if (index >= 0 && index < values.Count)
                    {
                        return values[index];
                    }
                }

                return null;
            }

            return new ExcelQuestionRowDto
            {
                RowNumber = rowNumber,
                Module = Get("module"),
                Exam = Get("exam", "examname") ?? string.Empty,
                Subject = Get("subject", "subjectname") ?? string.Empty,
                Topic = Get("topic", "topicname"),
                QuestionText = Get("questiontext", "question") ?? string.Empty,
                OptionA = Get("optiona"),
                OptionB = Get("optionb"),
                OptionC = Get("optionc"),
                OptionD = Get("optiond"),
                CorrectAnswer = (Get("correctanswer", "answer") ?? string.Empty).Trim(),
                Explanation = Get("explanation"),
                Marks = ParseDecimal(Get("marks"), 1m),
                NegativeMarks = ParseDecimal(Get("negativemarks"), 0m),
                DifficultyLevel = Get("difficultylevel", "difficulty") ?? "Medium",
                QuestionType = Get("questiontype") ?? "MCQ",
                QuestionImageUrl = Get("questionimageurl"),
                OptionAImageUrl = Get("optionaimageurl"),
                OptionBImageUrl = Get("optionbimageurl"),
                OptionCImageUrl = Get("optioncimageurl"),
                OptionDImageUrl = Get("optiondimageurl"),
                ExplanationImageUrl = Get("explanationimageurl"),
                Reference = Get("reference"),
                Tags = Get("tags"),
                SameExplanationForAllLanguages = ParseBool(Get("sameexplanationforalllanguages"))
            };
        }

        private static decimal ParseDecimal(string? input, decimal defaultValue)
        {
            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : defaultValue;
        }

        private static int FindHeaderIndex(IReadOnlyList<string> headers, string name)
        {
            for (var i = 0; i < headers.Count; i++)
            {
                if (string.Equals(headers[i], name, StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool ParseBool(string? input)
        {
            return bool.TryParse(input, out var parsed)
                ? parsed
                : string.Equals(input, "1", StringComparison.OrdinalIgnoreCase)
                  || string.Equals(input, "yes", StringComparison.OrdinalIgnoreCase)
                  || string.Equals(input, "y", StringComparison.OrdinalIgnoreCase);
        }

        // Exam Integration Methods
        public async Task<ExamNameDto> GetExamDetailsAsync(int examId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        e.Id,
                        e.Name,
                        e.ExamType,
                        e.SubjectId,
                        s.Name AS SubjectName,
                        e.HasNegativeMarking,
                        e.NegativeMarkingValue,
                        e.MarksPerQuestion
                    FROM Exams e
                    LEFT JOIN Subjects s ON e.SubjectId = s.Id
                    WHERE e.Id = @ExamId AND e.IsActive = 1";

                return await connection.QuerySingleOrDefaultAsync<ExamNameDto>(sql, new { ExamId = examId });
            });
        }

        public async Task<IEnumerable<ExamTypeDto>> GetExamTypesAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT DISTINCT 
                        et.Id,
                        et.Name,
                        et.ExamType
                    FROM ExamTypes et
                    WHERE et.IsActive = 1
                    ORDER BY et.Name";

                return await connection.QueryAsync<ExamTypeDto>(sql);
            });
        }

        public async Task<IEnumerable<ExamNameDto>> GetExamNamesByTypeAsync(string examType)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        e.Id,
                        e.Name,
                        e.ExamType,
                        e.SubjectId,
                        s.Name AS SubjectName,
                        e.HasNegativeMarking,
                        e.NegativeMarkingValue,
                        e.MarksPerQuestion
                    FROM Exams e
                    LEFT JOIN Subjects s ON e.SubjectId = s.Id
                    WHERE e.ExamType = @ExamType AND e.IsActive = 1
                    ORDER BY e.Name";

                return await connection.QueryAsync<ExamNameDto>(sql, new { ExamType = examType });
            });
        }

        public async Task<IEnumerable<ExamNameDto>> GetAllExamNamesAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        e.Id,
                        e.Name,
                        e.ExamType,
                        e.SubjectId,
                        s.Name AS SubjectName,
                        e.HasNegativeMarking,
                        e.NegativeMarkingValue,
                        e.MarksPerQuestion
                    FROM Exams e
                    LEFT JOIN Subjects s ON e.SubjectId = s.Id
                    WHERE e.IsActive = 1
                    ORDER BY e.ExamType, e.Name";

                return await connection.QueryAsync<ExamNameDto>(sql);
            });
        }
    }
}
