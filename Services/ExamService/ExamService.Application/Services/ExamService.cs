using AutoMapper;
using ExamService.Application.DTOs;
using ExamService.Application.Interfaces;
using ExamService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;

namespace ExamService.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IExamQualificationRepository _examQualificationRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration? _configuration;

        public ExamService(
            IExamRepository examRepository,
            IExamQualificationRepository examQualificationRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory,
            IConfiguration? configuration = null)
        {
            _examRepository = examRepository;
            _examQualificationRepository = examQualificationRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ExamDto> CreateExamAsync(CreateExamDto createDto)
        {
            var exam = _mapper.Map<Exam>(createDto);
            exam.CreatedAt = DateTime.UtcNow;
            exam.IsActive = true;

            await _examRepository.AddAsync(exam);
            await _examRepository.SaveChangesAsync();

            // Handle qualification and stream relationships
            if (createDto.QualificationIds?.Any() == true)
            {
                var examQualifications = new List<ExamQualification>();
                var streamIds = createDto.StreamIds ?? new List<int?>();
                
                for (int i = 0; i < createDto.QualificationIds.Count; i++)
                {
                    var qualificationId = createDto.QualificationIds[i];
                    var streamId = i < streamIds.Count ? streamIds[i] : null;
                    
                    examQualifications.Add(new ExamQualification
                    {
                        ExamId = exam.Id,
                        QualificationId = qualificationId,
                        StreamId = streamId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }

                await _examQualificationRepository.AddRangeAsync(examQualifications);
                await _examQualificationRepository.SaveChangesAsync();
            }

            var result = _mapper.Map<ExamDto>(exam);
            result.QualificationIds = createDto.QualificationIds ?? new List<int>();
            var examWithQuals = await _examRepository.GetByIdWithQualificationsAsync(exam.Id);
            result.StreamIds = examWithQuals?.ExamQualifications?.Select(eq => eq.StreamId).ToList() ?? new List<int?>();
            return result;
        }

        public async Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateDto)
        {
            var exam = await _examRepository.GetByIdWithQualificationsAsync(id);
            if (exam == null)
                return null;

            _mapper.Map(updateDto, exam);
            exam.UpdatedAt = DateTime.UtcNow;
            await _examRepository.UpdateAsync(exam);

            // Update qualification and stream relationships
            if (updateDto.QualificationIds != null)
            {
                // Remove all existing relationships
                var existingRelations = exam.ExamQualifications?.ToList() ?? new List<ExamQualification>();
                foreach (var eq in existingRelations)
                {
                    await _examQualificationRepository.DeleteAsync(eq);
                }

                // Add new relationships with streams
                var streamIds = updateDto.StreamIds ?? new List<int?>();
                for (int i = 0; i < updateDto.QualificationIds.Count; i++)
                {
                    var qualificationId = updateDto.QualificationIds[i];
                    var streamId = i < streamIds.Count ? streamIds[i] : null;
                    
                    await _examQualificationRepository.AddAsync(new ExamQualification
                    {
                        ExamId = exam.Id,
                        QualificationId = qualificationId,
                        StreamId = streamId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }

                await _examQualificationRepository.SaveChangesAsync();
            }

            await _examRepository.SaveChangesAsync();
            var updatedExam = await _examRepository.GetByIdWithQualificationsAsync(id);
            var result = _mapper.Map<ExamDto>(updatedExam);
            result.QualificationIds = updatedExam?.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
            return result;
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return false;

            await _examQualificationRepository.DeleteByExamIdAsync(id);
            await _examRepository.DeleteAsync(exam);
            await _examRepository.SaveChangesAsync();
            return true;
        }

        public async Task<ExamDto?> GetExamByIdAsync(int id)
        {
            var exam = await _examRepository.GetByIdWithQualificationsAsync(id);
            if (exam == null)
                return null;

            var examDto = _mapper.Map<ExamDto>(exam);
            examDto.QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
            examDto.StreamIds = exam.ExamQualifications?.Select(eq => eq.StreamId).ToList() ?? new List<int?>();
            return examDto;
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync(bool? isInternational = null)
        {
            var exams = await _examRepository.GetActiveAsync();
            var filteredExams = exams.Where(e => e.IsActive);
            
            if (isInternational.HasValue)
            {
                filteredExams = filteredExams.Where(e => e.IsInternational == isInternational.Value);
            }
            
            var examDtos = new List<ExamDto>();

            foreach (var exam in filteredExams.OrderBy(e => e.Name))
            {
                var examWithQuals = await _examRepository.GetByIdWithQualificationsAsync(exam.Id);
                var examDto = _mapper.Map<ExamDto>(examWithQuals ?? exam);
                examDto.QualificationIds = examWithQuals?.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                examDto.StreamIds = examWithQuals?.ExamQualifications?.Select(eq => eq.StreamId).ToList() ?? new List<int?>();
                examDtos.Add(examDto);
            }

            return examDtos;
        }

        public async Task<IEnumerable<ExamDto>> GetExamsByQualificationAsync(int qualificationId, int? streamId = null)
        {
            IEnumerable<Exam> exams;
            
            if (streamId.HasValue)
            {
                exams = await _examRepository.GetByQualificationAndStreamAsync(qualificationId, streamId);
            }
            else
            {
                exams = await _examRepository.GetByQualificationIdAsync(qualificationId);
            }
            
            var examDtos = new List<ExamDto>();

            foreach (var exam in exams.OrderBy(e => e.Name))
            {
                var examWithQuals = await _examRepository.GetByIdWithQualificationsAsync(exam.Id);
                var examDto = _mapper.Map<ExamDto>(examWithQuals ?? exam);
                examDto.QualificationIds = examWithQuals?.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                examDto.StreamIds = examWithQuals?.ExamQualifications?.Select(eq => eq.StreamId).ToList() ?? new List<int?>();
                examDtos.Add(examDto);
            }

            return examDtos;
        }

        public async Task<bool> ToggleExamStatusAsync(int id, bool isActive)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return false;

            exam.IsActive = isActive;
            exam.UpdatedAt = DateTime.UtcNow;
            await _examRepository.UpdateAsync(exam);
            await _examRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ExamDto>> GetExamsForUserAsync(int userId)
        {
            // Get user info from UserService to check international exam interest
            var httpClient = _httpClientFactory.CreateClient();
            var userServiceUrl = "http://localhost:5002"; // UserService URL
            
            try
            {
                // Call UserService to get user profile
                var response = await httpClient.GetAsync($"{userServiceUrl}/api/users/profile");
                
                // Add authentication header
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIzNiIsInN1YiI6IjM2IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIrOTE4ODg4ODg4ODg4Iiwicm9sZSI6IlVzZXIiLCJqdGkiOiI3NDVhZGY5My01N2Y1LTRkOWMtYmQxNi03ZDgzMzE4ZDdmM2YiLCJuYmYiOjE3NjkwNzQ5NzAsImV4cCI6MTc2OTA3ODU3MCwiaWF0IjoxNzY5MDc0OTcwLCJpc3MiOiJSYW5rVXBBUEkiLCJhdWQiOiJSYW5rVXBBUEkifQ.wwjT28Y0TLlzT1Fhn4vMiJoQ8o3WBQQHCKKXjFO6zk8");
                
                response = await httpClient.GetAsync($"{userServiceUrl}/api/users/profile");
                if (!response.IsSuccessStatusCode)
                {
                    // If user service fails, return Indian exams only (default behavior)
                    return await GetExamsByInternationalStatus(false);
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var userResponse = JsonSerializer.Deserialize<UserServiceResponse>(jsonContent, options);
                
                if (userResponse?.Data == null)
                {
                    return await GetExamsByInternationalStatus(false);
                }

                var isInterestedInInternational = userResponse.Data.InterestedInIntlExam;
                return await GetExamsByInternationalStatus(isInterestedInInternational);
            }
            catch (Exception ex)
            {
                // If any error occurs, return Indian exams only (safe default)
                Console.WriteLine($"Error calling UserService: {ex.Message}");
                return await GetExamsByInternationalStatus(false);
            }
        }

        private async Task<IEnumerable<ExamDto>> GetExamsByInternationalStatus(bool showInternational)
        {
            var allExams = await _examRepository.GetAllAsync();
            var filteredExams = allExams.Where(e => e.IsInternational == showInternational && e.IsActive);
            
            var examDtos = new List<ExamDto>();
            foreach (var exam in filteredExams.OrderBy(e => e.Name))
            {
                var examWithQuals = await _examRepository.GetByIdWithQualificationsAsync(exam.Id);
                var examDto = _mapper.Map<ExamDto>(examWithQuals ?? exam);
                examDto.QualificationIds = examWithQuals?.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                examDto.StreamIds = examWithQuals?.ExamQualifications?.Select(eq => eq.StreamId).ToList() ?? new List<int?>();
                examDtos.Add(examDto);
            }

            return examDtos;
        }

        public async Task<int> SeedInternationalExamsAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var qualificationServiceUrl = _configuration?["Services:QualificationService:BaseUrl"] ?? _configuration?["Services:MasterService:BaseUrl"] ?? "http://localhost:5009";
            
            try
            {
                // Get all qualifications from QualificationService
                var response = await httpClient.GetAsync($"{qualificationServiceUrl}/api/qualifications");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to fetch qualifications from QualificationService");
                    return 0;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var qualifications = JsonSerializer.Deserialize<List<QualificationDto>>(jsonContent, options);
                
                if (qualifications == null || !qualifications.Any())
                {
                    Console.WriteLine("No qualifications found");
                    return 0;
                }

                var createdCount = 0;
                var internationalExamNames = new[]
                {
                    "SAT", "IELTS", "TOEFL", "GRE", "GMAT", "PTE", "OET", "Duolingo English Test",
                    "Cambridge English", "LSAT", "MCAT", "ACT", "AP Exams", "IB Diploma", "FCE", "CAE"
                };

                // Create international exams for each qualification
                foreach (var qualification in qualifications.Where(q => q.IsActive))
                {
                    // Check if international exam already exists for this qualification
                    var existingExams = await _examRepository.GetByQualificationIdAsync(qualification.Id);
                    var hasInternationalExam = existingExams.Any(e => e.IsInternational);
                    
                    if (!hasInternationalExam)
                    {
                        // Create a new international exam
                        var randomExamName = internationalExamNames[createdCount % internationalExamNames.Length];
                        var createDto = new CreateExamDto
                        {
                            Name = $"{randomExamName} - {qualification.Name}",
                            Description = $"International {randomExamName} exam for {qualification.Name} qualification",
                            DurationInMinutes = 180, // 3 hours
                            TotalMarks = 100,
                            PassingMarks = 60,
                            IsInternational = true,
                            QualificationIds = new List<int> { qualification.Id }
                        };

                        await CreateExamAsync(createDto);
                        createdCount++;
                    }
                }

                return createdCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding international exams: {ex.Message}");
                return 0;
            }
        }
    }

    // Temporary class for UserService response deserialization
    public class UserServiceResponse
    {
        public UserData? Data { get; set; }
    }

    public class UserData
    {
        public bool InterestedInIntlExam { get; set; }
    }
}
