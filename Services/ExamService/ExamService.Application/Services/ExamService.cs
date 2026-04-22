using AutoMapper;
using ExamService.Application.DTOs;
using ExamService.Application.Interfaces;
using ExamService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ExamService.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration? _configuration;
        private static readonly HashSet<int> AllowedExamCategoryIds = new() { 1, 2, 3, 4 };
        private const int MockTestCategoryId = 2;
        private const int DeepPracticeCategoryId = 3;

        public ExamService(
            IExamRepository examRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory,
            IConfiguration? configuration = null)
        {
            _examRepository = examRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ExamDto> CreateExamAsync(CreateExamDto createDto, string? imageUrl = null)
        {
            ValidateExamPayload(createDto.Name, createDto.ExamCategoryId, createDto.SubjectId);

            var exam = _mapper.Map<Exam>(createDto);
            exam.CreatedAt = DateTime.UtcNow;
            exam.UpdatedAt = DateTime.UtcNow;
            exam.ImageUrl = imageUrl;
            ApplyScheduleState(exam);

            await _examRepository.AddAsync(exam);
            if (exam.Id <= 0)
            {
                throw new Exception("Exam creation failed - no valid ID returned");
            }

            var examDto = _mapper.Map<ExamDto>(exam);
            
            // Add subject name if subjectId exists
            if (exam.SubjectId.HasValue)
            {
                examDto.SubjectName = await GetSubjectNameById(exam.SubjectId.Value);
            }
            
            // Set exam type from createDto
            examDto.ExamType = createDto.ExamType;
            
            return examDto;
        }

        public async Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateDto)
        {
            var exam = await _examRepository.GetByIdWithQualificationsAsync(id);
            if (exam == null)
            {
                return null;
            }

            ValidateExamPayload(updateDto.Name, updateDto.ExamCategoryId, updateDto.SubjectId);
            _mapper.Map(updateDto, exam);
            exam.UpdatedAt = DateTime.UtcNow;
            ApplyScheduleState(exam);

            await _examRepository.UpdateAsync(exam);
            var updatedExam = await _examRepository.GetByIdWithQualificationsAsync(id);
            return updatedExam == null ? null : _mapper.Map<ExamDto>(updatedExam);
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
            {
                return false;
            }

            return await _examRepository.HardDeleteByIdAsync(id);
        }

        public async Task<ExamDto?> GetExamByIdAsync(int id)
        {
            var exam = await _examRepository.GetByIdWithQualificationsAsync(id);
            if (exam == null)
            {
                return null;
            }

            ApplyScheduleState(exam);
            return _mapper.Map<ExamDto>(exam);
        }

        public async Task<ExamDto?> GetExamByIdAsync(int id, string? language)
        {
            return await GetExamByIdAsync(id);
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync(bool? isInternational = null)
        {
            var exams = await _examRepository.GetActiveAsync();
            return BuildExamDtos(exams, isInternational);
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsIncludingInactiveAsync(bool? isInternational = null)
        {
            var exams = await _examRepository.GetAllIncludingInactiveAsync();
            return BuildExamDtos(exams, isInternational);
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsIncludingInactiveAsync(string? language, bool? isInternational = null)
        {
            var exams = await _examRepository.GetAllIncludingInactiveAsync();
            return BuildExamDtos(exams, isInternational);
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsForAdminAsync(bool? isInternational = null)
        {
            var exams = await _examRepository.GetAllIncludingInactiveAsync();
            return BuildExamDtos(exams, isInternational);
        }

        public async Task<bool> ToggleExamStatusAsync(int id, bool isActive)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
            {
                return false;
            }

            exam.IsActive = isActive;
            exam.Status = isActive ? "Active" : "Inactive";
            exam.UpdatedAt = DateTime.UtcNow;
            await _examRepository.UpdateAsync(exam);
            return true;
        }

        public async Task<IEnumerable<ExamDto>> GetExamsForUserAsync(int userId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var userServiceUrl = "http://localhost:5002";

            try
            {
                var response = await httpClient.GetAsync($"{userServiceUrl}/api/users/profile");
                if (!response.IsSuccessStatusCode)
                {
                    return await GetExamsByInternationalStatus(false);
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var userResponse = JsonSerializer.Deserialize<UserServiceResponse>(jsonContent, options);
                var isInterestedInInternational = userResponse?.Data?.InterestedInIntlExam ?? false;
                return await GetExamsByInternationalStatus(isInterestedInInternational);
            }
            catch
            {
                return await GetExamsByInternationalStatus(false);
            }
        }

        public async Task<int> SeedInternationalExamsAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var qualificationServiceUrl = _configuration?["Services:QualificationService:BaseUrl"] ?? _configuration?["Services:MasterService:BaseUrl"] ?? "http://localhost:5009";

            try
            {
                var response = await httpClient.GetAsync($"{qualificationServiceUrl}/api/qualifications");
                if (!response.IsSuccessStatusCode)
                {
                    return 0;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var qualifications = JsonSerializer.Deserialize<List<QualificationDto>>(jsonContent, options);
                if (qualifications == null || !qualifications.Any())
                {
                    return 0;
                }

                var createdCount = 0;
                var internationalExamNames = new[]
                {
                    "SAT", "IELTS", "TOEFL", "GRE", "GMAT", "PTE", "OET", "Duolingo English Test",
                    "Cambridge English", "LSAT", "MCAT", "ACT", "AP Exams", "IB Diploma", "FCE", "CAE"
                };

                foreach (var qualification in qualifications.Where(q => q.IsActive))
                {
                    var existingExams = await _examRepository.GetByQualificationIdAsync(qualification.Id);
                    if (existingExams.Any(e => e.IsInternational))
                    {
                        continue;
                    }

                    var randomExamName = internationalExamNames[createdCount % internationalExamNames.Length];
                    var createDto = new CreateExamDto
                    {
                        Name = $"{randomExamName} - {qualification.Name}",
                        Description = $"International {randomExamName} exam for {qualification.Name} qualification",
                        DurationInMinutes = 180,
                        TotalMarks = 100,
                        PassingMarks = 60,
                        IsInternational = true,
                        ExamCategoryId = 1
                    };

                    await CreateExamAsync(createDto);
                    createdCount++;
                }

                return createdCount;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error seeding international exams: {ex.Message}", ex);
            }
        }

        public async Task<ExamStatsDto> GetExamStatsAsync()
        {
            return await _examRepository.GetExamStatsAsync();
        }

        public async Task<bool> UpdateExamStatusAsync(int id, string status)
        {
            return await _examRepository.UpdateExamStatusAsync(id, status);
        }

        public async Task<ExamDashboardDto> GetExamDashboardAsync()
        {
            return await _examRepository.GetExamDashboardAsync();
        }

        private async Task<IEnumerable<ExamDto>> GetExamsByInternationalStatus(bool showInternational)
        {
            var allExams = await _examRepository.GetAllAsync();
            var filteredExams = allExams.Where(e => e.IsInternational == showInternational && e.IsActive);
            return BuildExamDtos(filteredExams, null);
        }

        private IEnumerable<ExamDto> BuildExamDtos(IEnumerable<Exam> exams, bool? isInternational)
        {
            if (isInternational.HasValue)
            {
                exams = exams.Where(e => e.IsInternational == isInternational.Value);
            }

            var examDtos = new List<ExamDto>();
            foreach (var exam in exams.OrderBy(e => e.Name))
            {
                ApplyScheduleState(exam);
                examDtos.Add(_mapper.Map<ExamDto>(exam));
            }

            return examDtos;
        }

        private static void ValidateExamPayload(string name, int? examCategoryId, int? subjectId)
        {
            Console.WriteLine($"ValidateExamPayload - Name: '{name}', ExamCategoryId: {examCategoryId}, SubjectId: {subjectId}");
            
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Exam name is required and cannot be empty.");
            }

            if (!examCategoryId.HasValue || !AllowedExamCategoryIds.Contains(examCategoryId.Value))
            {
                throw new ArgumentException("Exam category must be one of: Test Series, Mock Test, Deep Practice, Previous Year Question.");
            }

            if ((examCategoryId == MockTestCategoryId || examCategoryId == DeepPracticeCategoryId) && !subjectId.HasValue)
            {
                throw new ArgumentException("Subject is required for Mock Test and Deep Practice.");
            }
        }

        private static void ApplyScheduleState(Exam exam)
        {
            var now = DateTime.UtcNow;
            if (exam.ValidTill.HasValue && exam.ValidTill.Value <= now)
            {
                exam.IsActive = false;
                exam.Status = "Inactive";
                return;
            }

            if (exam.PublishDateTime.HasValue)
            {
                if (exam.PublishDateTime.Value <= now)
                {
                    exam.IsActive = true;
                    exam.Status = "Active";
                    return;
                }

                exam.IsActive = false;
                exam.Status = "Scheduled";
                return;
            }

            if (string.IsNullOrWhiteSpace(exam.Status))
            {
                exam.Status = "Draft";
            }

            if (string.Equals(exam.Status, "Draft", StringComparison.OrdinalIgnoreCase))
            {
                exam.IsActive = false;
            }
        }

        private async Task<string?> GetSubjectNameById(int subjectId)
        {
            try
            {
                // Mock subject names - in real implementation, this would come from database
                var subjectNames = new Dictionary<int, string>
                {
                    { 1, "Mathematics" },
                    { 2, "Physics" },
                    { 3, "Chemistry" },
                    { 4, "Biology" },
                    { 5, "English" },
                    { 6, "General Knowledge" },
                    { 7, "Reasoning" },
                    { 8, "Computer Science" }
                };

                return subjectNames.TryGetValue(subjectId, out var name) ? name : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> BulkHardDeleteExamsAsync(int[] excludedIds)
        {
            var allExams = await _examRepository.GetAllIncludingInactiveAsync();
            var examsToDelete = allExams.Where(e => !excludedIds.Contains(e.Id)).ToList();
            
            int deletedCount = 0;
            foreach (var exam in examsToDelete)
            {
                var result = await _examRepository.HardDeleteByIdAsync(exam.Id);
                if (result)
                    deletedCount++;
            }
            
            return deletedCount;
        }
    }

    public class UserServiceResponse
    {
        public UserData? Data { get; set; }
    }

    public class UserData
    {
        public bool InterestedInIntlExam { get; set; }
    }
}
