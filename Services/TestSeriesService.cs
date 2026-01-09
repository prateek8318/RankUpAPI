using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.DTOs;
using RankUpAPI.Models;
using RankUpAPI.Services.Interfaces;

namespace RankUpAPI.Services
{
    public class TestSeriesService : ITestSeriesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TestSeriesService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
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

            _context.TestSeries.Add(testSeries);
            await _context.SaveChangesAsync();

            return await GetTestSeriesByIdAsync(testSeries.Id) ?? throw new Exception("Failed to retrieve created test series");
        }

        public async Task<bool> DeleteTestSeriesAsync(int id)
        {
            var testSeries = await _context.TestSeries
                .Include(ts => ts.TestSeriesQuestions)
                .FirstOrDefaultAsync(ts => ts.Id == id);
            
            if (testSeries == null)
                return false;

            // Remove all test series questions
            _context.TestSeriesQuestions.RemoveRange(testSeries.TestSeriesQuestions);
            _context.TestSeries.Remove(testSeries);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TestSeriesDto>> GetAllTestSeriesAsync()
        {
            var testSeries = await _context.TestSeries
                .Include(ts => ts.Exam)
                .Where(ts => ts.IsActive)
                .OrderBy(ts => ts.DisplayOrder)
                .ThenBy(ts => ts.Name)
                .ToListAsync();

            var result = new List<TestSeriesDto>();
            foreach (var ts in testSeries)
            {
                var questionCount = await _context.TestSeriesQuestions
                    .CountAsync(tsq => tsq.TestSeriesId == ts.Id);

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
            var testSeries = await _context.TestSeries
                .Include(ts => ts.Exam)
                .FirstOrDefaultAsync(ts => ts.Id == id);

            if (testSeries == null)
                return null;

            // Get total questions count
            var questionCount = await _context.TestSeriesQuestions
                .CountAsync(tsq => tsq.TestSeriesId == id);

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
            var testSeries = await _context.TestSeries
                .Include(ts => ts.Exam)
                .Where(ts => ts.ExamId == examId && ts.IsActive)
                .OrderBy(ts => ts.DisplayOrder)
                .ThenBy(ts => ts.Name)
                .ToListAsync();

            var result = new List<TestSeriesDto>();
            foreach (var ts in testSeries)
            {
                var questionCount = await _context.TestSeriesQuestions
                    .CountAsync(tsq => tsq.TestSeriesId == ts.Id);

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
            var testSeries = await _context.TestSeries.FindAsync(id);
            if (testSeries == null)
                return false;

            testSeries.IsActive = isActive;
            testSeries.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TestSeriesDto?> UpdateTestSeriesAsync(int id, UpdateTestSeriesDto updateDto)
        {
            var testSeries = await _context.TestSeries.FindAsync(id);
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

            await _context.SaveChangesAsync();
            return await GetTestSeriesByIdAsync(id);
        }

        public async Task<bool> AddQuestionsToTestSeriesAsync(AddQuestionsToTestSeriesDto dto)
        {
            var testSeries = await _context.TestSeries.FindAsync(dto.TestSeriesId);
            if (testSeries == null)
                return false;

            // Get existing question IDs for this test series
            var existingQuestionIds = await _context.TestSeriesQuestions
                .Where(tsq => tsq.TestSeriesId == dto.TestSeriesId)
                .Select(tsq => tsq.QuestionId)
                .ToListAsync();

            // Filter out questions that already exist
            var newQuestionIds = dto.QuestionIds.Except(existingQuestionIds).ToList();

            // Check max 100 questions limit
            var currentQuestionCount = existingQuestionIds.Count;
            if (currentQuestionCount + newQuestionIds.Count > 100)
            {
                throw new InvalidOperationException($"Cannot add questions. Maximum 100 questions allowed. Current: {currentQuestionCount}, Trying to add: {newQuestionIds.Count}");
            }

            // Get the maximum order number
            var maxOrder = await _context.TestSeriesQuestions
                .Where(tsq => tsq.TestSeriesId == dto.TestSeriesId)
                .Select(tsq => tsq.QuestionOrder)
                .DefaultIfEmpty(0)
                .MaxAsync();

            // Add new questions (each question = 1 mark)
            foreach (var questionId in newQuestionIds)
            {
                var question = await _context.Questions.FindAsync(questionId);
                if (question != null)
                {
                    maxOrder++;
                    _context.TestSeriesQuestions.Add(new TestSeriesQuestion
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

            // Update total questions count (TotalMarks will be calculated automatically)
            var totalQuestions = await _context.TestSeriesQuestions
                .CountAsync(tsq => tsq.TestSeriesId == dto.TestSeriesId);
            testSeries.TotalQuestions = totalQuestions;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveQuestionFromTestSeriesAsync(int testSeriesId, int questionId)
        {
            var testSeriesQuestion = await _context.TestSeriesQuestions
                .FirstOrDefaultAsync(tsq => tsq.TestSeriesId == testSeriesId && tsq.QuestionId == questionId);

            if (testSeriesQuestion == null)
                return false;

            _context.TestSeriesQuestions.Remove(testSeriesQuestion);

            // Update total questions count (TotalMarks will be calculated automatically)
            var testSeries = await _context.TestSeries.FindAsync(testSeriesId);
            if (testSeries != null)
            {
                var totalQuestions = await _context.TestSeriesQuestions
                    .CountAsync(tsq => tsq.TestSeriesId == testSeriesId);
                testSeries.TotalQuestions = totalQuestions;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
