using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;

namespace MasterService.API.Controllers
{
    [ApiController]
    [Route("api/subjects")]
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllSubjects()
        {
            try
            {
                var subjects = await _subjectService.GetAllSubjectsAsync();
                return Ok(new
                {
                    success = true,
                    message = "Subjects retrieved successfully",
                    data = subjects
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subjects");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(id);
                if (subject == null)
                {
                    return NotFound(new { success = false, message = "Subject not found" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Subject retrieved successfully",
                    data = subject
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subject by id: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveSubjects()
        {
            try
            {
                var subjects = await _subjectService.GetActiveSubjectsAsync();
                return Ok(new
                {
                    success = true,
                    message = "Active subjects retrieved successfully",
                    data = subjects
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active subjects");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto createSubjectDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid request data", errors = ModelState });
                }

                var subject = await _subjectService.CreateSubjectAsync(createSubjectDto);
                return CreatedAtAction(
                    nameof(GetSubjectById),
                    new { id = subject.Id },
                    new
                    {
                        success = true,
                        message = "Subject created successfully",
                        data = subject
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subject");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] UpdateSubjectDto updateSubjectDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid request data", errors = ModelState });
                }

                var subject = await _subjectService.UpdateSubjectAsync(id, updateSubjectDto);
                return Ok(new
                {
                    success = true,
                    message = "Subject updated successfully",
                    data = subject
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subject with id: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            try
            {
                var result = await _subjectService.DeleteSubjectAsync(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Subject not found" });
                }

                return Ok(new { success = true, message = "Subject deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subject with id: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("exists/{id}")]
        public async Task<IActionResult> SubjectExists(int id)
        {
            try
            {
                var exists = await _subjectService.SubjectExistsAsync(id);
                return Ok(new { success = true, data = new { exists } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if subject exists with id: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}
