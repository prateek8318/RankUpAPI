using AutoMapper;
using TestService.Application.DTOs;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;

namespace TestService.Application.Services
{
    public class TestSeriesService
    {
        private readonly ITestSeriesRepository _repository;
        private readonly ITestRepository _testRepository;
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;

        public TestSeriesService(
            ITestSeriesRepository repository,
            ITestRepository testRepository,
            IExamRepository examRepository,
            IMapper mapper)
        {
            _repository = repository;
            _testRepository = testRepository;
            _examRepository = examRepository;
            _mapper = mapper;
        }

        public async Task<TestSeriesDto> CreateAsync(CreateTestSeriesDto dto)
        {
            // Validate exam exists
            var exam = await _examRepository.GetByIdAsync(dto.ExamId);
            if (exam == null)
                throw new KeyNotFoundException($"Exam with ID {dto.ExamId} not found");

            var testSeries = _mapper.Map<TestSeries>(dto);
            testSeries.CreatedAt = DateTime.UtcNow;
            testSeries.IsActive = true;

            await _repository.AddAsync(testSeries);
            await _repository.SaveChangesAsync();

            return await GetByIdAsync(testSeries.Id) ?? throw new InvalidOperationException("Failed to retrieve created test series");
        }

        public async Task<TestSeriesDto?> GetByIdAsync(int id)
        {
            var testSeries = await _repository.GetByIdWithTestsAsync(id);
            if (testSeries == null) return null;

            var dto = _mapper.Map<TestSeriesDto>(testSeries);
            
            if (testSeries.Exam != null)
                dto.ExamName = testSeries.Exam.Name;
            
            dto.TestCount = testSeries.Tests?.Count(t => t.IsActive) ?? 0;

            return dto;
        }

        public async Task<IEnumerable<TestSeriesDto>> GetAllAsync()
        {
            var testSeries = await _repository.GetActiveSeriesAsync();
            var dtos = _mapper.Map<IEnumerable<TestSeriesDto>>(testSeries).ToList();

            foreach (var dto in dtos)
            {
                var series = testSeries.FirstOrDefault(ts => ts.Id == dto.Id);
                if (series != null)
                {
                    if (series.Exam != null)
                        dto.ExamName = series.Exam.Name;
                    
                    dto.TestCount = series.Tests?.Count(t => t.IsActive) ?? 0;
                }
            }

            return dtos;
        }

        public async Task<IEnumerable<TestSeriesDto>> GetByExamIdAsync(int examId)
        {
            var testSeries = await _repository.GetByExamIdAsync(examId);
            var dtos = _mapper.Map<IEnumerable<TestSeriesDto>>(testSeries).ToList();

            foreach (var dto in dtos)
            {
                var series = testSeries.FirstOrDefault(ts => ts.Id == dto.Id);
                if (series != null)
                {
                    if (series.Exam != null)
                        dto.ExamName = series.Exam.Name;
                    
                    dto.TestCount = series.Tests?.Count(t => t.IsActive) ?? 0;
                }
            }

            return dtos;
        }

        public async Task<TestSeriesDto?> UpdateAsync(int id, UpdateTestSeriesDto dto)
        {
            var testSeries = await _repository.GetByIdAsync(id);
            if (testSeries == null) return null;

            // Validate exam exists
            var exam = await _examRepository.GetByIdAsync(dto.ExamId);
            if (exam == null)
                throw new KeyNotFoundException($"Exam with ID {dto.ExamId} not found");

            _mapper.Map(dto, testSeries);
            testSeries.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(testSeries);
            await _repository.SaveChangesAsync();

            return await GetByIdAsync(testSeries.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var testSeries = await _repository.GetByIdAsync(id);
            if (testSeries == null) return false;

            testSeries.IsActive = false;
            testSeries.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(testSeries);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
