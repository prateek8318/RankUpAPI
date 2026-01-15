using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;

namespace HomeDashboardService.API.Controllers
{
    /// <summary>
    /// Subject Management Controller (Admin)
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly ILogger<SubjectController> _logger;

        public SubjectController(
            ISubjectService subjectService,
            ILogger<SubjectController> logger)
        {
            _subjectService = subjectService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<SubjectDto>> CreateSubject([FromBody] CreateSubjectDto createSubjectDto)
        {
            try
            {
                var result = await _subjectService.CreateSubjectAsync(createSubjectDto);
                return CreatedAtAction(nameof(GetSubjectById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subject");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SubjectDto>> UpdateSubject(int id, [FromBody] UpdateSubjectDto updateSubjectDto)
        {
            try
            {
                var result = await _subjectService.UpdateSubjectAsync(id, updateSubjectDto);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subject: {SubjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSubject(int id)
        {
            try
            {
                var result = await _subjectService.DeleteSubjectAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subject: {SubjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/enable-disable")]
        public async Task<ActionResult> EnableDisableSubject(int id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _subjectService.EnableDisableSubjectAsync(id, isActive);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling/disabling subject: {SubjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDto>> GetSubjectById(int id)
        {
            try
            {
                var result = await _subjectService.GetSubjectByIdAsync(id);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subject: {SubjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("exam/{examId}")]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjectsByExamId(int examId)
        {
            try
            {
                var result = await _subjectService.GetSubjectsByExamIdAsync(examId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subjects for exam: {ExamId}", examId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
