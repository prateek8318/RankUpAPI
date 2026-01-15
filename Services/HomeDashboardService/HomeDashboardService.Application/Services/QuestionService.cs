using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace HomeDashboardService.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionOptionRepository _questionOptionRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IBulkUploadLogRepository _bulkUploadLogRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(
            IQuestionRepository questionRepository,
            IQuestionOptionRepository questionOptionRepository,
            IQuizRepository quizRepository,
            IBulkUploadLogRepository bulkUploadLogRepository,
            IMapper mapper,
            ILogger<QuestionService> logger)
        {
            _questionRepository = questionRepository;
            _questionOptionRepository = questionOptionRepository;
            _quizRepository = quizRepository;
            _bulkUploadLogRepository = bulkUploadLogRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto createQuestionDto)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(createQuestionDto.QuizId);
                if (quiz == null)
                    throw new KeyNotFoundException($"Quiz with ID {createQuestionDto.QuizId} not found");

                if (createQuestionDto.Options.Count < 2)
                    throw new ArgumentException("Question must have at least 2 options");

                var hasCorrectAnswer = createQuestionDto.Options.Any(o => o.IsCorrect);
                if (!hasCorrectAnswer)
                    throw new ArgumentException("Question must have at least one correct answer");

                var question = new Question
                {
                    QuizId = createQuestionDto.QuizId,
                    QuestionText = createQuestionDto.QuestionText,
                    ImageUrl = createQuestionDto.ImageUrl,
                    VideoUrl = createQuestionDto.VideoUrl,
                    Explanation = createQuestionDto.Explanation,
                    Difficulty = createQuestionDto.Difficulty,
                    Marks = createQuestionDto.Marks,
                    DisplayOrder = createQuestionDto.DisplayOrder
                };

                var createdQuestion = await _questionRepository.AddAsync(question);
                await _questionRepository.SaveChangesAsync();

                // Add options
                foreach (var optionDto in createQuestionDto.Options)
                {
                    var option = new QuestionOption
                    {
                        QuestionId = createdQuestion.Id,
                        OptionText = optionDto.OptionText,
                        ImageUrl = optionDto.ImageUrl,
                        IsCorrect = optionDto.IsCorrect,
                        DisplayOrder = optionDto.DisplayOrder
                    };
                    await _questionOptionRepository.AddAsync(option);
                }

                await _questionOptionRepository.SaveChangesAsync();

                var result = await _questionRepository.GetByIdWithOptionsAsync(createdQuestion.Id);
                return _mapper.Map<QuestionDto>(result!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                throw;
            }
        }

        public async Task<QuestionDto?> UpdateQuestionAsync(int id, UpdateQuestionDto updateQuestionDto)
        {
            try
            {
                var question = await _questionRepository.GetByIdWithOptionsAsync(id);
                if (question == null)
                    return null;

                question.QuestionText = updateQuestionDto.QuestionText;
                question.ImageUrl = updateQuestionDto.ImageUrl;
                question.VideoUrl = updateQuestionDto.VideoUrl;
                question.Explanation = updateQuestionDto.Explanation;
                question.Difficulty = updateQuestionDto.Difficulty;
                question.Marks = updateQuestionDto.Marks;
                question.DisplayOrder = updateQuestionDto.DisplayOrder;
                question.UpdatedAt = DateTime.UtcNow;

                // Delete existing options
                var existingOptions = question.Options.ToList();
                foreach (var option in existingOptions)
                {
                    option.IsActive = false;
                    option.UpdatedAt = DateTime.UtcNow;
                    await _questionOptionRepository.UpdateAsync(option);
                }

                // Add new options
                foreach (var optionDto in updateQuestionDto.Options)
                {
                    var option = new QuestionOption
                    {
                        QuestionId = question.Id,
                        OptionText = optionDto.OptionText,
                        ImageUrl = optionDto.ImageUrl,
                        IsCorrect = optionDto.IsCorrect,
                        DisplayOrder = optionDto.DisplayOrder
                    };
                    await _questionOptionRepository.AddAsync(option);
                }

                await _questionRepository.UpdateAsync(question);
                await _questionRepository.SaveChangesAsync();
                await _questionOptionRepository.SaveChangesAsync();

                var updated = await _questionRepository.GetByIdWithOptionsAsync(id);
                return updated != null ? _mapper.Map<QuestionDto>(updated) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question: {QuestionId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            try
            {
                return await _questionRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question: {QuestionId}", id);
                throw;
            }
        }

        public async Task<bool> EnableDisableQuestionAsync(int id, bool isActive)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(id);
                if (question == null)
                    return false;

                question.IsActive = isActive;
                question.UpdatedAt = DateTime.UtcNow;
                await _questionRepository.UpdateAsync(question);
                await _questionRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling/disabling question: {QuestionId}", id);
                throw;
            }
        }

        public async Task<QuestionDto?> GetQuestionByIdAsync(int id)
        {
            try
            {
                var question = await _questionRepository.GetByIdWithOptionsAsync(id);
                return question != null ? _mapper.Map<QuestionDto>(question) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting question: {QuestionId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionDto>> GetQuestionsByQuizIdAsync(int quizId)
        {
            try
            {
                var questions = await _questionRepository.GetByQuizIdAsync(quizId);
                return _mapper.Map<IEnumerable<QuestionDto>>(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting questions for quiz: {QuizId}", quizId);
                throw;
            }
        }

        public async Task<BulkUploadResultDto> BulkUploadQuestionsAsync(int quizId, Stream fileStream, string fileName, int userId)
        {
            var bulkUploadLog = new BulkUploadLog
            {
                FileName = fileName,
                Status = BulkUploadStatus.Processing,
                ProcessedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var errors = new List<BulkUploadErrorDto>();
            var questions = new List<Question>();

            try
            {
                var quiz = await _quizRepository.GetByIdAsync(quizId);
                if (quiz == null)
                {
                    throw new KeyNotFoundException($"Quiz with ID {quizId} not found");
                }

                // Read CSV file
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim
                };

                using var reader = new StreamReader(fileStream, Encoding.UTF8);
                using var csv = new CsvReader(reader, csvConfig);

                var records = csv.GetRecords<QuestionCsvRow>().ToList();
                bulkUploadLog.TotalRows = records.Count;

                int rowNumber = 1;
                foreach (var row in records)
                {
                    rowNumber++;
                    try
                    {
                        var validationErrors = ValidateQuestionRow(row, rowNumber);
                        if (validationErrors.Any())
                        {
                            errors.AddRange(validationErrors);
                            continue;
                        }

                        var question = new Question
                        {
                            QuizId = quizId,
                            QuestionText = row.QuestionText!,
                            ImageUrl = string.IsNullOrWhiteSpace(row.ImageUrl) ? null : row.ImageUrl,
                            VideoUrl = string.IsNullOrWhiteSpace(row.VideoUrl) ? null : row.VideoUrl,
                            Explanation = string.IsNullOrWhiteSpace(row.Explanation) ? null : row.Explanation,
                            Difficulty = ParseDifficulty(row.Difficulty),
                            Marks = int.TryParse(row.Marks, out var marks) ? marks : 1,
                            DisplayOrder = rowNumber - 1
                        };

                        await _questionRepository.AddAsync(question);
                        await _questionRepository.SaveChangesAsync();

                        // Add options
                        var options = ParseOptions(row, question.Id);
                        foreach (var option in options)
                        {
                            await _questionOptionRepository.AddAsync(option);
                        }
                        await _questionOptionRepository.SaveChangesAsync();

                        questions.Add(question);
                        bulkUploadLog.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new BulkUploadErrorDto
                        {
                            RowNumber = rowNumber,
                            FieldName = "General",
                            ErrorMessage = ex.Message,
                            RowData = System.Text.Json.JsonSerializer.Serialize(row)
                        });
                        bulkUploadLog.ErrorCount++;
                    }
                }

                bulkUploadLog.Status = errors.Any() 
                    ? (questions.Any() ? BulkUploadStatus.PartialSuccess : BulkUploadStatus.Failed)
                    : BulkUploadStatus.Completed;
                bulkUploadLog.ProcessedAt = DateTime.UtcNow;
                bulkUploadLog.ErrorSummary = errors.Any() 
                    ? $"Found {errors.Count} errors in {bulkUploadLog.TotalRows} rows"
                    : "All rows processed successfully";

                await _bulkUploadLogRepository.AddAsync(bulkUploadLog);
                await _bulkUploadLogRepository.SaveChangesAsync();

                return new BulkUploadResultDto
                {
                    BulkUploadLogId = bulkUploadLog.Id,
                    TotalRows = bulkUploadLog.TotalRows,
                    SuccessCount = bulkUploadLog.SuccessCount,
                    ErrorCount = bulkUploadLog.ErrorCount,
                    Status = bulkUploadLog.Status,
                    ErrorSummary = bulkUploadLog.ErrorSummary,
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk upload");
                bulkUploadLog.Status = BulkUploadStatus.Failed;
                bulkUploadLog.ProcessedAt = DateTime.UtcNow;
                bulkUploadLog.ErrorSummary = ex.Message;
                await _bulkUploadLogRepository.AddAsync(bulkUploadLog);
                await _bulkUploadLogRepository.SaveChangesAsync();

                throw;
            }
        }

        public async Task<BulkUploadLogDto?> GetBulkUploadLogByIdAsync(int id)
        {
            try
            {
                var log = await _bulkUploadLogRepository.GetByIdAsync(id);
                return log != null ? _mapper.Map<BulkUploadLogDto>(log) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bulk upload log: {LogId}", id);
                throw;
            }
        }

        public async Task<List<BulkUploadErrorDto>> GetBulkUploadErrorsAsync(int bulkUploadLogId)
        {
            // Errors are stored in the BulkUploadLog.ErrorSummary or can be retrieved from the error report file
            // For simplicity, returning empty list. In production, you'd parse the error report file
            return new List<BulkUploadErrorDto>();
        }

        private List<BulkUploadErrorDto> ValidateQuestionRow(QuestionCsvRow row, int rowNumber)
        {
            var errors = new List<BulkUploadErrorDto>();

            if (string.IsNullOrWhiteSpace(row.QuestionText))
            {
                errors.Add(new BulkUploadErrorDto
                {
                    RowNumber = rowNumber,
                    FieldName = nameof(row.QuestionText),
                    ErrorMessage = "Question text is required"
                });
            }

            if (string.IsNullOrWhiteSpace(row.Option1) || string.IsNullOrWhiteSpace(row.Option2))
            {
                errors.Add(new BulkUploadErrorDto
                {
                    RowNumber = rowNumber,
                    FieldName = "Options",
                    ErrorMessage = "At least 2 options are required"
                });
            }

            var hasCorrectAnswer = new[] { row.CorrectAnswer }.Any(ca => 
                ca == "1" || ca == "2" || ca == "3" || ca == "4");
            if (!hasCorrectAnswer)
            {
                errors.Add(new BulkUploadErrorDto
                {
                    RowNumber = rowNumber,
                    FieldName = nameof(row.CorrectAnswer),
                    ErrorMessage = "Correct answer must be 1, 2, 3, or 4"
                });
            }

            return errors;
        }

        private QuestionDifficulty ParseDifficulty(string? difficulty)
        {
            if (string.IsNullOrWhiteSpace(difficulty))
                return QuestionDifficulty.Medium;

            return difficulty.ToLower() switch
            {
                "easy" => QuestionDifficulty.Easy,
                "medium" => QuestionDifficulty.Medium,
                "hard" => QuestionDifficulty.Hard,
                _ => QuestionDifficulty.Medium
            };
        }

        private List<QuestionOption> ParseOptions(QuestionCsvRow row, int questionId)
        {
            var options = new List<QuestionOption>();
            int correctAnswer = int.TryParse(row.CorrectAnswer, out var ca) ? ca : 1;

            if (!string.IsNullOrWhiteSpace(row.Option1))
            {
                options.Add(new QuestionOption
                {
                    QuestionId = questionId,
                    OptionText = row.Option1,
                    IsCorrect = correctAnswer == 1,
                    DisplayOrder = 1
                });
            }

            if (!string.IsNullOrWhiteSpace(row.Option2))
            {
                options.Add(new QuestionOption
                {
                    QuestionId = questionId,
                    OptionText = row.Option2,
                    IsCorrect = correctAnswer == 2,
                    DisplayOrder = 2
                });
            }

            if (!string.IsNullOrWhiteSpace(row.Option3))
            {
                options.Add(new QuestionOption
                {
                    QuestionId = questionId,
                    OptionText = row.Option3,
                    IsCorrect = correctAnswer == 3,
                    DisplayOrder = 3
                });
            }

            if (!string.IsNullOrWhiteSpace(row.Option4))
            {
                options.Add(new QuestionOption
                {
                    QuestionId = questionId,
                    OptionText = row.Option4,
                    IsCorrect = correctAnswer == 4,
                    DisplayOrder = 4
                });
            }

            return options;
        }
    }

    // CSV Row mapping class
    public class QuestionCsvRow
    {
        public string? QuestionText { get; set; }
        public string? Option1 { get; set; }
        public string? Option2 { get; set; }
        public string? Option3 { get; set; }
        public string? Option4 { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public string? Difficulty { get; set; }
        public string? Marks { get; set; }
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
    }
}
