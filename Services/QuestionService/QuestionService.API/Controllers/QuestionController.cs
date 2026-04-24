using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Application.DTOs;
using QuestionService.Application.Services;
using QuestionService.Application.Interfaces;
using QuestionApplicationService = QuestionService.Application.Services.QuestionService;
using QuestionService.API.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace QuestionService.API.Controllers
{
    [Route("api/questions")]
    [ApiController]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private readonly QuestionApplicationService _service;

        public QuestionController(QuestionApplicationService service)
        {
            _service = service;
        }

        private int GetAuthenticatedUserId()
        {
            return AuthClaimsHelper.GetUserId(User);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAll([FromQuery] int? examId = null, [FromQuery] int? subjectId = null, [FromQuery] int? topicId = null, [FromQuery] string? difficultyLevel = null, [FromQuery] bool? isPublished = null, [FromQuery] string languageCode = "en", [FromQuery] int pageSize = 20, [FromQuery] string? cursor = null, [FromQuery] string direction = "next")
        {
            var request = new QuestionCursorRequestDto
            {
                ExamId = examId,
                SubjectId = subjectId,
                TopicId = topicId,
                DifficultyLevel = difficultyLevel,
                IsPublished = isPublished,
                LanguageCode = languageCode,
                PageSize = pageSize,
                Cursor = cursor,
                Direction = direction
            };

            var result = await _service.GetQuestionsCursorAsync(request);
            return Ok(new { 
                data = result.Data,
                nextCursor = result.NextCursor,
                previousCursor = result.PreviousCursor,
                hasNextPage = result.HasNextPage,
                hasPreviousPage = result.HasPreviousPage,
                totalCount = result.TotalCount,
                pageSize = result.PageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetById(int id)
        {
            var question = await _service.GetByIdAsync(id);
            if (question == null)
                return NotFound();
            return Ok(question);
        }

        [HttpGet("chapter/{chapterId}")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetByChapter(int chapterId)
        {
            var questions = await _service.GetByTopicIdAsync(chapterId);
            return Ok(questions);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QuestionDto>> Create(CreateQuestionDto dto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                dto.CreatedBy = userId;
                var question = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = question.Id }, question);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = "Invalid question payload." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Failed to create question." });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateQuestionDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var result = await _service.UpdateAsync(id, dto);
            if (result == null)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("admin/topics")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateTopic([FromBody] CreateTopicDto dto)
        {
            var id = await _service.CreateTopicAsync(dto);
            return Ok(new { success = true, topicId = id });
        }

        [HttpGet("admin/topics")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<TopicDto>>> GetTopics([FromQuery] int? subjectId = null, [FromQuery] int? examId = null, [FromQuery] bool includeInactive = true)
        {
            var result = await _service.GetTopicsAsync(subjectId ?? 0);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateAdminQuestion([FromForm] CreateQuestionFormDataDto? dto)
        {
            if (dto == null)
                return BadRequest(new { success = false, message = "Request form data is required." });

            var userId = GetAuthenticatedUserId();
            if (userId <= 0)
                return Unauthorized(new { success = false, message = "Invalid user" });

            dto.CreatedBy = userId;
            var question = await _service.CreateAdminQuestionWithImagesAsync(dto);
            return Ok(new { success = true, questionId = question.Id, data = question });
        }

        // Keep the old JSON endpoint for backward compatibility
        [HttpPost("admin/json")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateAdminQuestionJson([FromBody] CreateQuestionRequestDto dto)
        {
            var userId = GetAuthenticatedUserId();
            if (userId <= 0)
                return Unauthorized(new { success = false, message = "Invalid user" });

            dto.CreatedBy = userId;
            var question = await _service.CreateAdminQuestionAsync(dto);
            return Ok(new { success = true, questionId = question.Id, data = question });
        }

        [HttpPut("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> UpdateAdminQuestion(int id, [FromBody] UpdateQuestionAdminDto dto)
        {
            dto.Id = id;
            var updated = await _service.UpdateAdminQuestionAsync(dto);
            return Ok(new { success = true, data = updated });
        }

        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QuestionAdminDetailDto>> GetAdminQuestionById(int id)
        {
            var result = await _service.GetAdminQuestionByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { success = false, message = "Question not found." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpGet("admin/paged")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QuestionPagedResponseDto>> GetAdminQuestionsPaged([FromQuery] QuestionFilterRequestDto filter)
        {
            var result = await _service.GetAdminQuestionsPagedAsync(filter);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("admin/mock-test/{mockTestId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QuestionPagedResponseDto>> GetQuestionsByMockTest(int mockTestId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] bool includeInactive = true)
        {
            var filter = new QuestionFilterRequestDto
            {
                MockTestId = mockTestId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                IncludeInactive = includeInactive
            };
            var result = await _service.GetAdminQuestionsPagedAsync(filter);
            return Ok(new { success = true, data = result });
        }

        [HttpPatch("admin/{id}/publish")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> SetPublishStatus(int id, [FromBody] PublishQuestionRequestDto dto)
        {
            dto.QuestionId = id;
            var updated = await _service.SetPublishStatusAsync(dto);
            if (!updated)
            {
                return NotFound(new { success = false, message = "Question not found." });
            }

            return Ok(new { success = true, message = "Publish status updated." });
        }

        [HttpGet("admin/dashboard/stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QuestionDashboardStatsDto>> GetDashboardStats()
        {
            var result = await _service.GetDashboardStatsAsync();
            return Ok(new { success = true, data = result });
        }

        [HttpPost("admin/bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> BulkUpload([FromBody] BulkQuestionUploadRequestDto request)
        {
            var result = await _service.BulkCreateQuestionsAsync(request);
            return Ok(new { success = true, data = result });
        }

        // Image upload endpoints
        [HttpPost("admin/upload-image")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ImageUploadResponseDto>> UploadQuestionImage([FromForm] QuestionImageUploadDto dto)
        {
            var result = await _service.UploadQuestionImageAsync(dto);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        // Quiz functionality endpoints
        [HttpPost("quiz/start")]
        public async Task<ActionResult<QuizSessionDto>> StartQuiz([FromBody] QuizStartRequestDto dto)
        {
            try
            {
                var tokenUserId = GetAuthenticatedUserId();
                if (tokenUserId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid token user." });
                if (dto.UserId != tokenUserId)
                    return Unauthorized(new { success = false, message = "Invalid user for quiz session." });

                var session = await _service.StartQuizAsync(dto);
                return Ok(new { success = true, data = session });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Unable to start quiz with provided payload." });
            }
        }

        [HttpGet("quiz/{sessionId}")]
        public async Task<ActionResult<QuizSessionDto>> GetQuizSession(int sessionId, [FromQuery] int userId)
        {
            var tokenUserId = GetAuthenticatedUserId();
            if (tokenUserId <= 0)
                return Unauthorized(new { success = false, message = "Invalid token user." });
            if (userId != tokenUserId)
                return Unauthorized(new { success = false, message = "Invalid user for quiz session." });

            var session = await _service.GetQuizSessionAsync(sessionId, tokenUserId);
            if (session == null)
                return NotFound(new { success = false, message = "Quiz session not found" });
            
            return Ok(new { success = true, data = session });
        }

        [HttpPost("quiz/save-answer")]
        public async Task<ActionResult<object>> SaveQuizAnswer([FromBody] QuizAnswerRequestDto dto)
        {
            try
            {
                var tokenUserId = GetAuthenticatedUserId();
                if (tokenUserId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid token user." });
                var session = await _service.GetQuizSessionAsync(dto.QuizSessionId, tokenUserId);
                if (session == null)
                    return Forbid();

                var result = await _service.SaveQuizAnswerAsync(dto);
                return Ok(new { success = true, saved = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Unable to save quiz answer with provided payload." });
            }
        }

        [HttpPost("quiz/submit")]
        public async Task<ActionResult<QuizResultDto>> SubmitQuiz([FromBody] QuizSubmitRequestDto dto)
        {
            try
            {
                var tokenUserId = GetAuthenticatedUserId();
                if (tokenUserId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid token user." });
                var session = await _service.GetQuizSessionAsync(dto.QuizSessionId, tokenUserId);
                if (session == null)
                    return Forbid();

                var result = await _service.SubmitQuizAsync(dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Unable to submit quiz with provided payload." });
            }
        }

        // Subject and Exam listing endpoints
        [HttpGet("subjects")]
        public async Task<ActionResult<IEnumerable<SubjectListDto>>> GetSubjects()
        {
            var subjects = await _service.GetSubjectsAsync();
            return Ok(new { success = true, data = subjects });
        }

        [HttpGet("exams")]
        public async Task<ActionResult<IEnumerable<ExamListDto>>> GetExams([FromQuery] int? subjectId = null)
        {
            var exams = await _service.GetExamsAsync(subjectId);
            return Ok(new { success = true, data = exams });
        }

        // Bulk upload status endpoints
        [HttpGet("admin/bulk/{batchId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BulkUploadProcessDto>> GetBulkUploadStatus(int batchId)
        {
            var status = await _service.GetBulkUploadStatusAsync(batchId);
            if (status == null)
                return NotFound(new { success = false, message = "Batch not found" });
            
            return Ok(new { success = true, data = status });
        }

        [HttpPost("admin/bulk/parse")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ExcelQuestionRowDto>>> ParseBulkUploadFile([FromForm] IFormFile file)
        {
            try
            {
                var extension = Path.GetExtension(file.FileName);
                var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var questions = await _service.ParseBulkUploadFileAsync(tempPath);
                
                // Clean up temp file
                System.IO.File.Delete(tempPath);
                
                return Ok(new { success = true, data = questions });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Unable to parse bulk upload file." });
            }
        }

        // Simple Question Creation with Exam Integration
        [HttpPost("simple")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateSimpleQuestion([FromBody] SimpleQuestionCreateDto dto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                dto.CreatedBy = userId;
                var question = await _service.CreateSimpleQuestionAsync(dto);
                return Ok(new { success = true, questionId = question.Id, data = question });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Unable to create question with provided payload." });
            }
        }

        // Get Exam Types from ExamService
        [HttpGet("exam-types")]
        public async Task<ActionResult<object>> GetExamTypes()
        {
            try
            {
                var examTypes = await _service.GetExamTypesAsync();
                return Ok(new { success = true, data = examTypes });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Unable to fetch exam types." });
            }
        }

        // Get Exam Names by ExamType from ExamService
        [HttpGet("exam-names/{examType}")]
        public async Task<ActionResult<object>> GetExamNames(string examType)
        {
            try
            {
                var examNames = await _service.GetExamNamesByTypeAsync(examType);
                return Ok(new { success = true, data = examNames });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Unable to fetch exam names for requested type." });
            }
        }

        // Get All Exam Names
        [HttpGet("exam-names")]
        public async Task<ActionResult<object>> GetAllExamNames()
        {
            try
            {
                var examNames = await _service.GetAllExamNamesAsync();
                return Ok(new { success = true, data = examNames });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Unable to fetch exam names." });
            }
        }

        // Create Question with Images
        [HttpPost("admin/with-images")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateQuestionWithImages([FromForm] CreateQuestionWithImagesDto dto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                dto.CreatedBy = userId;
                var question = await _service.CreateQuestionWithImagesAsync(dto);
                return Ok(new { success = true, questionId = question.Id, data = question });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Unable to create question with images." });
            }
        }

        // Update Question with Images
        [HttpPut("admin/{id}/with-images")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> UpdateQuestionWithImages(int id, [FromForm] UpdateQuestionWithImagesDto dto)
        {
            try
            {
                dto.Id = id;
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var question = await _service.UpdateQuestionWithImagesAsync(dto);
                return Ok(new { success = true, data = question });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Unable to update question with images." });
            }
        }
    }
}
