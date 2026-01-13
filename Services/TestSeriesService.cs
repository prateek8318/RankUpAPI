using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.DTOs;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;
using RankUpAPI.Services.Interfaces;

namespace RankUpAPI.Services
{
    public class TestSeriesService : ITestSeriesService
    {
        private readonly ITestSeriesRepository _testSeriesRepository;
        private readonly ITestSeriesQuestionRepository _testSeriesQuestionRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;

        public TestSeriesService(ITestSeriesRepository testSeriesRepository, ITestSeriesQuestionRepository testSeriesQuestionRepository, IQuestionRepository questionRepository, IMapper mapper)
        {
            _testSeriesRepository = testSeriesRepository;
            _testSeriesQuestionRepository = testSeriesQuestionRepository;
            _questionRepository = questionRepository;
            _mapper = mapper;
        }

        public async Task<TestSeriesDto> CreateTestSeriesAsync(CreateTestSeriesDto createDto)
        {
            var testSeries = _mapper.Map<TestSeries>(createDto);
            testSeries.CreatedAt = DateTime.UtcNow;
            testSeries.IsActive = true;
            testSeries.TotalQuestions = 0;
            testSeries.TotalMarks = 0; // Will be calculated from questions (1 question = 1 mark)
            testSeries.PassingMarks = 0; // Can be set later if needed

            await _testSeriesRepository.AddAsync(testSeries);
            await _testSeriesRepository.SaveChangesAsync();

            return await GetTestSeriesByIdAsync(testSeries.Id) ?? throw new Exception("Failed to retrieve created test series");
        }

        public async Task<bool> DeleteTestSeriesAsync(int id)
        {
            var testSeries = await _testSeriesRepository.GetByIdWithDetailsAsync(id);
            
            if (testSeries == null)
                return false;

            // Remove all test series questions
            await _testSeriesQuestionRepository.DeleteByTestSeriesIdAsync(id);
            await _testSeriesRepository.DeleteAsync(testSeries);
            await _testSeriesRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TestSeriesDto>> GetAllTestSeriesAsync()
        {
            var testSeries = await _testSeriesRepository.GetActiveAsync();

            var result = new List<TestSeriesDto>();
            foreach (var ts in testSeries)
            {
                var questionCount = await _testSeriesQuestionRepository.GetQuestionCountByTestSeriesIdAsync(ts.Id);

                result.Add(new TestSeriesDto
                {
                    Id = ts.Id,
                    Name = ts.Name,
                    Description = ts.Description,
                    ExamId = ts.ExamId,
                    ExamName = ts.Exam.Name,
                    DurationInMinutes = ts.DurationInMinutes,
                    TotalMarks = questionCount, // 1 question = 1 mark
                    TotalQuestions = questionCount,
                    PassingMarks = ts.PassingMarks,
                    InstructionsEnglish = ts.InstructionsEnglish,
                    InstructionsHindi = ts.InstructionsHindi,
                    DisplayOrder = ts.DisplayOrder,
                    IsLocked = ts.IsLocked,
                    IsActive = ts.IsActive,
                    CreatedAt = ts.CreatedAt,
                    UpdatedAt = ts.UpdatedAt
                });
            }
            return result;
        }

        public async Task<TestSeriesDto?> GetTestSeriesByIdAsync(int id)
        {
            var testSeries = await _testSeriesRepository.GetByIdWithDetailsAsync(id);

            if (testSeries == null)
                return null;

            // Get total questions count
            var questionCount = await _testSeriesQuestionRepository.GetQuestionCountByTestSeriesIdAsync(id);

            // TotalMarks = TotalQuestions (1 question = 1 mark)
            return new TestSeriesDto
            {
                Id = testSeries.Id,
                Name = testSeries.Name,
                Description = testSeries.Description,
                ExamId = testSeries.ExamId,
                ExamName = testSeries.Exam.Name,
                DurationInMinutes = testSeries.DurationInMinutes,
                TotalMarks = questionCount, // 1 question = 1 mark
                TotalQuestions = questionCount,
                PassingMarks = testSeries.PassingMarks,
                InstructionsEnglish = testSeries.InstructionsEnglish,
                InstructionsHindi = testSeries.InstructionsHindi,
                DisplayOrder = testSeries.DisplayOrder,
                IsLocked = testSeries.IsLocked,
                IsActive = testSeries.IsActive,
                CreatedAt = testSeries.CreatedAt,
                UpdatedAt = testSeries.UpdatedAt
            };
        }

        public async Task<IEnumerable<TestSeriesDto>> GetTestSeriesByExamIdAsync(int examId)
        {
            var testSeries = await _testSeriesRepository.GetByExamIdAsync(examId);

            var result = new List<TestSeriesDto>();
            foreach (var ts in testSeries)
            {
                var questionCount = await _testSeriesQuestionRepository.GetQuestionCountByTestSeriesIdAsync(ts.Id);

                result.Add(new TestSeriesDto
                {
                    Id = ts.Id,
                    Name = ts.Name,
                    Description = ts.Description,
                    ExamId = ts.ExamId,
                    ExamName = ts.Exam.Name,
                    DurationInMinutes = ts.DurationInMinutes,
                    TotalMarks = questionCount, // 1 question = 1 mark
                    TotalQuestions = questionCount,
                    PassingMarks = ts.PassingMarks,
                    InstructionsEnglish = ts.InstructionsEnglish,
                    InstructionsHindi = ts.InstructionsHindi,
                    DisplayOrder = ts.DisplayOrder,
                    IsLocked = ts.IsLocked,
                    IsActive = ts.IsActive,
                    CreatedAt = ts.CreatedAt,
                    UpdatedAt = ts.UpdatedAt
                });
            }

            return result;
        }

        public async Task<bool> ToggleTestSeriesStatusAsync(int id, bool isActive)
        {
            var testSeries = await _testSeriesRepository.GetByIdAsync(id);
            if (testSeries == null)
                return false;

            testSeries.IsActive = isActive;
            testSeries.UpdatedAt = DateTime.UtcNow;
            await _testSeriesRepository.UpdateAsync(testSeries);
            await _testSeriesRepository.SaveChangesAsync();
            return true;
        }

        public async Task<TestSeriesDto?> UpdateTestSeriesAsync(int id, UpdateTestSeriesDto updateDto)
        {
            var testSeries = await _testSeriesRepository.GetByIdAsync(id);
            if (testSeries == null)
                return null;

            testSeries.Name = updateDto.Name;
            testSeries.Description = updateDto.Description;
            testSeries.DurationInMinutes = updateDto.DurationInMinutes;
            // TotalMarks will be calculated from TotalQuestions automatically
            testSeries.InstructionsEnglish = updateDto.InstructionsEnglish;
            testSeries.InstructionsHindi = updateDto.InstructionsHindi;
            testSeries.DisplayOrder = updateDto.DisplayOrder;
            testSeries.IsLocked = updateDto.IsLocked;
            testSeries.IsActive = updateDto.IsActive;
            testSeries.UpdatedAt = DateTime.UtcNow;

            await _testSeriesRepository.UpdateAsync(testSeries);
            await _testSeriesRepository.SaveChangesAsync();
            return await GetTestSeriesByIdAsync(id);
        }

        public async Task<bool> AddQuestionsToTestSeriesAsync(AddQuestionsToTestSeriesDto dto)
        {
            var testSeries = await _testSeriesRepository.GetByIdAsync(dto.TestSeriesId);
            if (testSeries == null)
                return false;

            // Get existing question IDs for this test series
            var existingQuestions = await _testSeriesQuestionRepository.GetByTestSeriesIdAsync(dto.TestSeriesId);
            var existingQuestionIds = existingQuestions.Select(tsq => tsq.QuestionId).ToList();

            // Filter out questions that already exist
            var newQuestionIds = dto.QuestionIds.Except(existingQuestionIds).ToList();

            // Check max 100 questions limit
            var currentQuestionCount = existingQuestionIds.Count;
            if (currentQuestionCount + newQuestionIds.Count > 100)
            {
                throw new InvalidOperationException($"Cannot add questions. Maximum 100 questions allowed. Current: {currentQuestionCount}, Trying to add: {newQuestionIds.Count}");
            }

            // Get the maximum order number
            var maxOrder = await _testSeriesQuestionRepository.GetMaxOrderByTestSeriesIdAsync(dto.TestSeriesId);

            // Add new questions (each question = 1 mark)
            var testSeriesQuestions = new List<TestSeriesQuestion>();
            foreach (var questionId in newQuestionIds)
            {
                var question = await _questionRepository.GetByIdAsync(questionId);
                if (question != null)
                {
                    maxOrder++;
                    testSeriesQuestions.Add(new TestSeriesQuestion
                    {
                        TestSeriesId = dto.TestSeriesId,
                        QuestionId = questionId,
                        QuestionOrder = maxOrder,
                        Marks = 1, // Each question = 1 mark
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            if (testSeriesQuestions.Any())
            {
                await _testSeriesQuestionRepository.AddRangeAsync(testSeriesQuestions);
            }

            // Update total questions count (TotalMarks will be calculated automatically)
            var totalQuestions = await _testSeriesQuestionRepository.GetQuestionCountByTestSeriesIdAsync(dto.TestSeriesId);
            testSeries.TotalQuestions = totalQuestions;

            await _testSeriesRepository.UpdateAsync(testSeries);
            await _testSeriesRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveQuestionFromTestSeriesAsync(int testSeriesId, int questionId)
        {
            var testSeriesQuestion = await _testSeriesQuestionRepository.GetByTestSeriesAndQuestionIdAsync(testSeriesId, questionId);

            if (testSeriesQuestion == null)
                return false;

            await _testSeriesQuestionRepository.DeleteAsync(testSeriesQuestion);

            // Update total questions count (TotalMarks will be calculated automatically)
            var testSeries = await _testSeriesRepository.GetByIdAsync(testSeriesId);
            if (testSeries != null)
            {
                var totalQuestions = await _testSeriesQuestionRepository.GetQuestionCountByTestSeriesIdAsync(testSeriesId);
                testSeries.TotalQuestions = totalQuestions;
                await _testSeriesRepository.UpdateAsync(testSeries);
            }

            await _testSeriesRepository.SaveChangesAsync();
            return true;
        }
    }
}
