using AutoMapper;
using TestService.Application.DTOs;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;

namespace TestService.Application.Services
{
    public class HomeDashboardService
    {
        private readonly IExamRepository _examRepository;
        private readonly IPracticeModeRepository _practiceModeRepository;
        private readonly ITestRepository _testRepository;
        private readonly IUserTestAttemptRepository _userTestAttemptRepository;
        private readonly IMapper _mapper;

        public HomeDashboardService(
            IExamRepository examRepository,
            IPracticeModeRepository practiceModeRepository,
            ITestRepository testRepository,
            IUserTestAttemptRepository userTestAttemptRepository,
            IMapper mapper)
        {
            _examRepository = examRepository;
            _practiceModeRepository = practiceModeRepository;
            _testRepository = testRepository;
            _userTestAttemptRepository = userTestAttemptRepository;
            _mapper = mapper;
        }

        public async Task<HomeDashboardResponseDto> GetHomeDashboardAsync(int userId)
        {
            var response = new HomeDashboardResponseDto();

            // Get selected exam (first active exam as default - in production, this would come from user profile)
            var activeExams = await _examRepository.GetActiveExamsAsync();
            var selectedExam = activeExams.FirstOrDefault();
            
            if (selectedExam != null)
            {
                response.SelectedExam = new SelectedExamDto
                {
                    Id = selectedExam.Id,
                    Name = selectedExam.Name,
                    Description = selectedExam.Description,
                    IconUrl = selectedExam.IconUrl
                };
            }

            // Get practice modes
            var practiceModes = await _practiceModeRepository.GetActiveModesAsync();
            response.PracticeModes = _mapper.Map<List<PracticeModeDto>>(practiceModes);

            // Get resume tests (ongoing attempts)
            var ongoingAttempts = await _userTestAttemptRepository.GetOngoingByUserIdAsync(userId);
            response.ResumeTests = ongoingAttempts.Select(attempt => new ResumeTestDto
            {
                Id = attempt.Test.Id,
                Title = attempt.Test.Title,
                PracticeModeName = attempt.Test.PracticeMode.Name,
                ProgressPercentage = attempt.Test.TotalQuestions > 0 
                    ? (int)((attempt.CurrentQuestionIndex * 100.0) / attempt.Test.TotalQuestions)
                    : 0,
                StartedAt = attempt.StartedAt ?? DateTime.UtcNow,
                TimeRemainingSeconds = Math.Max(0, (attempt.Test.DurationInMinutes * 60) - (int)(DateTime.UtcNow - (attempt.StartedAt ?? DateTime.UtcNow)).TotalSeconds),
                SeriesName = attempt.Test.Series?.Name,
                SubjectName = attempt.Test.Subject?.Name
            }).ToList();

            // Get daily targets (placeholder - would integrate with a separate service)
            response.DailyTargets = new List<DailyTargetDto>
            {
                new DailyTargetDto
                {
                    Id = 1,
                    Title = "Daily Practice Goal",
                    Description = "Complete 30 minutes of practice",
                    TargetMinutes = 30,
                    CompletedMinutes = 15,
                    ProgressPercentage = 50,
                    IsCompleted = false
                }
            };

            return response;
        }

        public async Task<IEnumerable<PracticeModeDto>> GetPracticeModesAsync()
        {
            var practiceModes = await _practiceModeRepository.GetActiveModesAsync();
            return _mapper.Map<IEnumerable<PracticeModeDto>>(practiceModes);
        }

        public async Task<IEnumerable<ExamDto>> GetExamsAsync()
        {
            var exams = await _examRepository.GetActiveExamsAsync();
            return _mapper.Map<IEnumerable<ExamDto>>(exams);
        }
    }

    public class HomeDashboardResponseDto
    {
        public SelectedExamDto? SelectedExam { get; set; }
        public List<PracticeModeDto> PracticeModes { get; set; } = new();
        public List<ResumeTestDto> ResumeTests { get; set; } = new();
        public List<DailyTargetDto> DailyTargets { get; set; } = new();
    }
}
