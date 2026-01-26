using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using System.Text.Json;
using TestService.Application.DTOs;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;

namespace TestService.Application.Services
{
    public class TestService
    {
        private readonly ITestRepository _testRepository;
        private readonly ITestSeriesRepository _testSeriesRepository;
        private readonly IExamRepository _examRepository;
        private readonly IPracticeModeRepository _practiceModeRepository;
        private readonly IMapper _mapper;

        public TestService(
            ITestRepository testRepository,
            ITestSeriesRepository testSeriesRepository,
            IExamRepository examRepository,
            IPracticeModeRepository practiceModeRepository,
            IMapper mapper)
        {
            _testRepository = testRepository;
            _testSeriesRepository = testSeriesRepository;
            _examRepository = examRepository;
            _practiceModeRepository = practiceModeRepository;
            _mapper = mapper;
        }

        public async Task<TestDto?> GetByIdAsync(int id)
        {
            var test = await _testRepository.GetByIdWithQuestionsAsync(id);
            if (test == null) return null;

            var testDto = _mapper.Map<TestDto>(test);
            
            // Load related data
            if (test.Exam != null)
                testDto.ExamName = test.Exam.Name;
            
            if (test.PracticeMode != null)
                testDto.PracticeModeName = test.PracticeMode.Name;
            
            if (test.Series != null)
                testDto.SeriesName = test.Series.Name;
            
            if (test.Subject != null)
                testDto.SubjectName = test.Subject.Name;

            return testDto;
        }

        public async Task<IEnumerable<TestDto>> GetTestsAsync(TestFilterDto filter)
        {
            var tests = await _testRepository.GetByExamAndPracticeModeWithFiltersAsync(
                filter.ExamId, 
                filter.PracticeModeId, 
                filter.SeriesId, 
                filter.SubjectId, 
                filter.Year);

            var testDtos = _mapper.Map<IEnumerable<TestDto>>(tests).ToList();

            // Load related data for each test
            foreach (var testDto in testDtos)
            {
                var test = tests.FirstOrDefault(t => t.Id == testDto.Id);
                if (test != null)
                {
                    if (test.Exam != null)
                        testDto.ExamName = test.Exam.Name;
                    
                    if (test.PracticeMode != null)
                        testDto.PracticeModeName = test.PracticeMode.Name;
                    
                    if (test.Series != null)
                        testDto.SeriesName = test.Series.Name;
                    
                    if (test.Subject != null)
                        testDto.SubjectName = test.Subject.Name;
                }
            }

            return testDtos;
        }

        public async Task<TestDto> CreateAsync(CreateTestDto dto)
        {
            // Validate practice mode and filters
            await ValidateTestFilters(dto.PracticeModeId, dto.SeriesId, dto.SubjectId, dto.Year);

            var test = _mapper.Map<Test>(dto);
            test.CreatedAt = DateTime.UtcNow;
            test.IsActive = true;

            await _testRepository.AddAsync(test);
            await _testRepository.SaveChangesAsync();

            return await GetByIdAsync(test.Id) ?? throw new InvalidOperationException("Failed to retrieve created test");
        }

        public async Task<TestDto?> UpdateAsync(int id, UpdateTestDto dto)
        {
            // Validate practice mode and filters
            await ValidateTestFilters(dto.PracticeModeId, dto.SeriesId, dto.SubjectId, dto.Year);

            var test = await _testRepository.GetByIdAsync(id);
            if (test == null) return null;

            _mapper.Map(dto, test);
            test.UpdatedAt = DateTime.UtcNow;

            await _testRepository.UpdateAsync(test);
            await _testRepository.SaveChangesAsync();

            return await GetByIdAsync(test.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var test = await _testRepository.GetByIdAsync(id);
            if (test == null) return false;

            test.IsActive = false;
            test.UpdatedAt = DateTime.UtcNow;

            await _testRepository.UpdateAsync(test);
            await _testRepository.SaveChangesAsync();

            return true;
        }

        public async Task<TestBulkUploadResultDto> BulkUploadTestsAsync(int seriesId, Stream fileStream, string fileName, int userId)
        {
            var result = new TestBulkUploadResultDto
            {
                SeriesId = seriesId,
                Status = "Processing"
            };

            try
            {
                // Validate series exists
                var series = await _testSeriesRepository.GetByIdAsync(seriesId);
                if (series == null)
                {
                    throw new KeyNotFoundException($"Test Series with ID {seriesId} not found");
                }

                // Read CSV file
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim
                };

                using var reader = new StreamReader(fileStream, Encoding.UTF8);
                using var csv = new CsvReader(reader, csvConfig);

                var records = csv.GetRecords<TestBulkUploadDto>().ToList();
                result.TotalRows = records.Count;

                int rowNumber = 1;
                foreach (var record in records)
                {
                    rowNumber++;
                    try
                    {
                        var validationErrors = ValidateBulkUploadRow(record, rowNumber, series);
                        if (validationErrors.Any())
                        {
                            result.Errors.AddRange(validationErrors);
                            result.ErrorCount++;
                            continue;
                        }

                        var test = new Test
                        {
                            ExamId = series.ExamId,
                            PracticeModeId = PracticeModeIds.TestSeries,
                            SeriesId = seriesId,
                            Title = record.TestTitle,
                            Description = record.Description,
                            DurationInMinutes = record.DurationInMinutes,
                            TotalQuestions = record.TotalQuestions,
                            TotalMarks = record.TotalMarks,
                            PassingMarks = record.PassingMarks,
                            InstructionsEnglish = record.InstructionsEnglish,
                            InstructionsHindi = record.InstructionsHindi,
                            DisplayOrder = record.DisplayOrder,
                            IsLocked = false,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        await _testRepository.AddAsync(test);
                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(new TestBulkUploadErrorDto
                        {
                            RowNumber = rowNumber,
                            FieldName = "General",
                            ErrorMessage = ex.Message,
                            RowData = System.Text.Json.JsonSerializer.Serialize(record)
                        });
                        result.ErrorCount++;
                    }
                }

                await _testRepository.SaveChangesAsync();

                result.Status = result.ErrorCount > 0 
                    ? (result.SuccessCount > 0 ? "PartialSuccess" : "Failed")
                    : "Completed";

                return result;
            }
            catch (Exception ex)
            {
                result.Status = "Failed";
                result.Errors.Add(new TestBulkUploadErrorDto
                {
                    RowNumber = 0,
                    FieldName = "General",
                    ErrorMessage = ex.Message
                });
                return result;
            }
        }

        private async Task ValidateTestFilters(int practiceModeId, int? seriesId, int? subjectId, int? year)
        {
            var practiceMode = await _practiceModeRepository.GetByIdAsync(practiceModeId);
            if (practiceMode == null)
                throw new KeyNotFoundException($"Practice Mode with ID {practiceModeId} not found");

            switch (practiceModeId)
            {
                case PracticeModeIds.MockTest:
                    if (seriesId.HasValue || subjectId.HasValue || year.HasValue)
                        throw new ArgumentException("Mock Test should not have Series, Subject, or Year filters");
                    break;

                case PracticeModeIds.TestSeries:
                    if (!seriesId.HasValue)
                        throw new ArgumentException("Test Series requires SeriesId");
                    if (subjectId.HasValue || year.HasValue)
                        throw new ArgumentException("Test Series should not have Subject or Year filters");
                    
                    var series = await _testSeriesRepository.GetByIdAsync(seriesId.Value);
                    if (series == null)
                        throw new KeyNotFoundException($"Test Series with ID {seriesId} not found");
                    break;

                case PracticeModeIds.DeepPractice:
                    if (seriesId.HasValue || year.HasValue)
                        throw new ArgumentException("Deep Practice should not have Series or Year filters");
                    if (!subjectId.HasValue)
                        throw new ArgumentException("Deep Practice requires SubjectId");
                    break;

                case PracticeModeIds.PreviousYear:
                    if (seriesId.HasValue || subjectId.HasValue)
                        throw new ArgumentException("Previous Year should not have Series or Subject filters");
                    if (!year.HasValue)
                        throw new ArgumentException("Previous Year requires Year");
                    break;

                default:
                    throw new ArgumentException($"Invalid Practice Mode ID: {practiceModeId}");
            }
        }

        private List<TestBulkUploadErrorDto> ValidateBulkUploadRow(TestBulkUploadDto row, int rowNumber, TestSeries series)
        {
            var errors = new List<TestBulkUploadErrorDto>();

            if (string.IsNullOrWhiteSpace(row.TestTitle))
            {
                errors.Add(new TestBulkUploadErrorDto
                {
                    RowNumber = rowNumber,
                    FieldName = nameof(row.TestTitle),
                    ErrorMessage = "Test title is required"
                });
            }

            if (row.DurationInMinutes <= 0)
            {
                errors.Add(new TestBulkUploadErrorDto
                {
                    RowNumber = rowNumber,
                    FieldName = nameof(row.DurationInMinutes),
                    ErrorMessage = "Duration must be greater than 0"
                });
            }

            if (row.TotalQuestions < 0)
            {
                errors.Add(new TestBulkUploadErrorDto
                {
                    RowNumber = rowNumber,
                    FieldName = nameof(row.TotalQuestions),
                    ErrorMessage = "Total questions cannot be negative"
                });
            }

            if (row.TotalMarks <= 0)
            {
                errors.Add(new TestBulkUploadErrorDto
                {
                    RowNumber = rowNumber,
                    FieldName = nameof(row.TotalMarks),
                    ErrorMessage = "Total marks must be greater than 0"
                });
            }

            if (row.PassingMarks < 0 || row.PassingMarks > row.TotalMarks)
            {
                errors.Add(new TestBulkUploadErrorDto
                {
                    RowNumber = rowNumber,
                    FieldName = nameof(row.PassingMarks),
                    ErrorMessage = "Passing marks must be between 0 and total marks"
                });
            }

            return errors;
        }
    }
}
