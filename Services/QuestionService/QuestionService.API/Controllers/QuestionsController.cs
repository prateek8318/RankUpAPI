using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Application.DTOs;
using QuestionService.Application.Services;
using QuestionService.API.Helpers;
using System.Security.Claims;

namespace QuestionService.API.Controllers
{
    /// <summary>
    /// Questions Management Controller
    /// </summary>
    [Route("api/question-management")]
    [ApiController]
    [Authorize]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly ILogger<QuestionsController> _logger;

        public QuestionsController(IQuestionService questionService, ILogger<QuestionsController> logger)
        {
            _questionService = questionService;
            _logger = logger;
        }

        private int GetAuthenticatedUserId()
        {
            return AuthClaimsHelper.GetUserId(User);
        }

        /// <summary>
        /// Create a new question
        /// </summary>
        /// <param name="createDto">Question creation details</param>
        /// <returns>Created question</returns>
        [HttpPost]
        public async Task<ActionResult<QuestionDto>> Create([FromBody] CreateQuestionDto createDto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized("Invalid user");

                createDto.CreatedBy = userId;
                var result = await _questionService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get question by ID
        /// </summary>
        /// <param name="id">Question ID</param>
        /// <returns>Question details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetById(int id)
        {
            try
            {
                var result = await _questionService.GetByIdAsync(id);
                if (result == null)
                    return NotFound($"Question with ID {id} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question: {QuestionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all questions
        /// </summary>
        /// <returns>All questions</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAll()
        {
            try
            {
                var result = await _questionService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all questions");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get paginated questions with filters
        /// </summary>
        /// <param name="request">Pagination and filter parameters</param>
        /// <returns>Paginated questions</returns>
        [HttpGet("paged")]
        public async Task<ActionResult<QuestionListResponseDto>> GetPaged([FromQuery] QuestionListRequestDto request)
        {
            try
            {
                var result = await _questionService.GetPagedAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated questions");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get questions by exam ID
        /// </summary>
        /// <param name="examId">Exam ID</param>
        /// <returns>Questions for the exam</returns>
        [HttpGet("exam/{examId}")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetByExamId(int examId)
        {
            try
            {
                var result = await _questionService.GetByExamIdAsync(examId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions for exam: {ExamId}", examId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get questions by subject ID
        /// </summary>
        /// <param name="subjectId">Subject ID</param>
        /// <returns>Questions for the subject</returns>
        [HttpGet("subject/{subjectId}")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetBySubjectId(int subjectId)
        {
            try
            {
                var result = await _questionService.GetBySubjectIdAsync(subjectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions for subject: {SubjectId}", subjectId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get questions by topic ID
        /// </summary>
        /// <param name="topicId">Topic ID</param>
        /// <returns>Questions for the topic</returns>
        [HttpGet("topic/{topicId}")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetByTopicId(int topicId)
        {
            try
            {
                var result = await _questionService.GetByTopicIdAsync(topicId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions for topic: {TopicId}", topicId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update question
        /// </summary>
        /// <param name="id">Question ID</param>
        /// <param name="updateDto">Question update details</param>
        /// <returns>Updated question</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<QuestionDto>> Update(int id, [FromBody] UpdateQuestionDto updateDto)
        {
            try
            {
                var result = await _questionService.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Question with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question: {QuestionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete question (soft delete)
        /// </summary>
        /// <param name="id">Question ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var success = await _questionService.DeleteAsync(id);
                if (!success)
                    return NotFound($"Question with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question: {QuestionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Publish question
        /// </summary>
        /// <param name="request">Publish request</param>
        /// <returns>No content</returns>
        [HttpPost("publish")]
        public async Task<ActionResult> PublishQuestion([FromBody] PublishQuestionDto request)
        {
            try
            {
                var success = await _questionService.PublishQuestionAsync(request.QuestionId, request.ReviewedBy);
                if (!success)
                    return NotFound($"Question with ID {request.QuestionId} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing question: {QuestionId}", request.QuestionId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Unpublish question
        /// </summary>
        /// <param name="request">Unpublish request</param>
        /// <returns>No content</returns>
        [HttpPost("unpublish")]
        public async Task<ActionResult> UnpublishQuestion([FromBody] ToggleQuestionPublishStatusDto request)
        {
            try
            {
                var success = await _questionService.UnpublishQuestionAsync(request.QuestionId, request.ReviewedBy);
                if (!success)
                    return NotFound($"Question with ID {request.QuestionId} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing question: {QuestionId}", request.QuestionId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get question statistics
        /// </summary>
        /// <returns>Question statistics</returns>
        [HttpGet("statistics")]
        public async Task<ActionResult<QuestionStatisticsDto>> GetStatistics()
        {
            try
            {
                var result = await _questionService.GetStatisticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get question by transaction ID
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>Question details</returns>
        [HttpGet("transaction/{transactionId}")]
        public async Task<ActionResult<QuestionDto>> GetByTransactionId(string transactionId)
        {
            try
            {
                var result = await _questionService.GetByTransactionIdAsync(transactionId);
                if (result == null)
                    return NotFound($"Question with transaction ID {transactionId} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question by transaction ID: {TransactionId}", transactionId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
