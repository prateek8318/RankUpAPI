using ExamService.Application.DTOs;
using ExamService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Services;

namespace ExamService.API.Controllers
{
    /// <summary>
    /// Public Exam Controller for Users
    /// </summary>
    [Route("api/exams")]
    [ApiController]
    [AllowAnonymous]
    public class UserExamController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly ILanguageService _languageService;

        public UserExamController(IExamService examService, ILanguageService languageService)
        {
            _examService = examService;
            _languageService = languageService;
        }

        /// <summary>
        /// Get all active exams for users
        /// </summary>
        /// <param name="isInternational">Optional filter for international exams</param>
        /// <param name="language">Optional language code</param>
        /// <returns>List of active exams only</returns>
        [HttpGet]
        public async Task<ActionResult<object>> GetActiveExams(
            [FromQuery] bool? isInternational = null,
            [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var exams = await _examService.GetAllExamsAsync(isInternational);
                
                return Ok(new
                {
                    success = true,
                    data = exams,
                    language = currentLanguage,
                    message = "Active exams fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error fetching active exams" });
            }
        }

        /// <summary>
        /// Get active exams filtered by category/subject/type
        /// </summary>
        /// <param name="examCategoryId">1=TestSeries, 2=MockTest, 3=DeepPractice, 4=PreviousYear</param>
        /// <param name="subjectId">Subject ID filter</param>
        /// <param name="examTypeId">Exam type filter</param>
        /// <param name="isInternational">Optional international filter</param>
        /// <param name="language">Optional language code</param>
        [HttpGet("by-flow")]
        public async Task<ActionResult<object>> GetActiveExamsByFlow(
            [FromQuery] int? examCategoryId = null,
            [FromQuery] int? subjectId = null,
            [FromQuery] int? examTypeId = null,
            [FromQuery] bool? isInternational = null,
            [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var exams = await _examService.GetActiveExamsByFiltersAsync(examCategoryId, subjectId, examTypeId, isInternational);

                return Ok(new
                {
                    success = true,
                    data = exams,
                    language = currentLanguage,
                    message = "Filtered exams fetched successfully"
                });
            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Error fetching filtered exams" });
            }
        }

        /// <summary>
        /// Get active exam by ID for users
        /// </summary>
        /// <param name="id">Exam ID</param>
        /// <param name="language">Optional language code</param>
        /// <returns>Active exam details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetActiveExam(int id, [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var exam = await _examService.GetExamByIdAsync(id);
                
                if (exam == null || !exam.IsActive || exam.Status != "Active")
                    return NotFound(new { success = false, message = "Active exam not found" });

                return Ok(new
                {
                    success = true,
                    data = exam,
                    language = currentLanguage,
                    message = "Active exam fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error fetching active exam" });
            }
        }
    }
}
