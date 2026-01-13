using AutoMapper;
using QuizService.Application.DTOs;
using QuizService.Application.Interfaces;
using QuizService.Domain.Entities;

namespace QuizService.Application.Services
{
    public class TestSeriesService
    {
        private readonly ITestSeriesRepository _repository;
        private readonly IMapper _mapper;

        public TestSeriesService(ITestSeriesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TestSeriesDto> CreateAsync(CreateTestSeriesDto dto)
        {
            var testSeries = _mapper.Map<TestSeries>(dto);
            testSeries.CreatedAt = DateTime.UtcNow;
            testSeries.IsActive = true;

            await _repository.AddAsync(testSeries);
            await _repository.SaveChangesAsync();

            return _mapper.Map<TestSeriesDto>(testSeries);
        }

        public async Task<TestSeriesDto?> GetByIdAsync(int id)
        {
            var testSeries = await _repository.GetByIdAsync(id);
            return testSeries == null ? null : _mapper.Map<TestSeriesDto>(testSeries);
        }

        public async Task<IEnumerable<TestSeriesDto>> GetAllAsync()
        {
            var testSeries = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TestSeriesDto>>(testSeries);
        }

        public async Task<IEnumerable<TestSeriesDto>> GetByExamIdAsync(int examId)
        {
            var testSeries = await _repository.GetByExamIdAsync(examId);
            return _mapper.Map<IEnumerable<TestSeriesDto>>(testSeries);
        }

        public async Task<TestSeriesDto?> UpdateAsync(int id, UpdateTestSeriesDto dto)
        {
            var testSeries = await _repository.GetByIdAsync(id);
            if (testSeries == null)
                return null;

            _mapper.Map(dto, testSeries);
            testSeries.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(testSeries);
            await _repository.SaveChangesAsync();

            return _mapper.Map<TestSeriesDto>(testSeries);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var testSeries = await _repository.GetByIdAsync(id);
            if (testSeries == null)
                return false;

            await _repository.DeleteAsync(testSeries);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
