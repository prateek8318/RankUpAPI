using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Common.Services;
using Common.Language;
using ILanguageService = Common.Services.ILanguageService;
using ILanguageDataService = Common.Language.ILanguageDataService;

namespace MasterService.API.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly ILogger<SubjectController> _logger;
        private readonly ILanguageService _languageService;

        public SubjectController(
            ISubjectService subjectService,
            ILogger<SubjectController> logger,
            ILanguageService languageService)
        {
            _subjectService = subjectService;
            _logger = logger;
            _languageService = languageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllSubjects([FromQuery] int? languageId = null)
        {
            try
            {
                if (!languageId.HasValue)
                {
                    languageId = GetLanguageIdFromHeader();
                }

                var subjects = await _subjectService.GetAllSubjectsAsync(languageId);
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
        public async Task<IActionResult> GetSubjectById(int id, [FromQuery] int? languageId = null)
        {
            try
            {
                if (!languageId.HasValue)
                {
                    languageId = GetLanguageIdFromHeader();
                }

                var subject = await _subjectService.GetSubjectByIdAsync(id, languageId);
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
        public async Task<IActionResult> GetActiveSubjects([FromQuery] int? languageId = null)
        {
            try
            {
                if (!languageId.HasValue)
                {
                    languageId = GetLanguageIdFromHeader();
                }

                var subjects = await _subjectService.GetActiveSubjectsAsync(languageId);
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
            catch (SqlException ex) when (ex.Number == 50000 || ex.Number == 2601 || ex.Number == 2627)
            {
                // 50000: RAISERROR from stored procedure (validation/business rule)
                // 2601/2627: unique constraint violations
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
            catch (SqlException ex) when (ex.Number == 50000 || ex.Number == 2601 || ex.Number == 2627)
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

        [HttpPatch("{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleSubjectStatus(int id, [FromBody] ToggleStatusDto toggleStatusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid request data", errors = ModelState });
                }

                var result = await _subjectService.ToggleSubjectStatusAsync(id, toggleStatusDto.IsActive);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Subject not found" });
                }

                return Ok(new { success = true, message = $"Subject {(toggleStatusDto.IsActive ? "activated" : "deactivated")} successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling subject status with id: {Id}", id);
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

        private int? GetLanguageIdFromHeader()
        {
            var code = _languageService.GetCurrentLanguage();
            return code switch
            {
                "en" => 50, // English
                "hi" => 49, // Hindi
                "ta" => null, // Tamil - add ID when available
                "gu" => null, // Gujarati - add ID when available
                _ => 50 // Default to English
            };
        }
    }
}
