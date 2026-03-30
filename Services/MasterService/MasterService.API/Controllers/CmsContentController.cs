using CommonLanguageService = Common.Services.ILanguageService;
using MasterService.Application.DTOs;
using MasterService.Application.Exceptions;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterService.API.Controllers
{
    [Route("api/cms")]
    [ApiController]
    public class CmsContentController : ControllerBase
    {
        private readonly ICmsContentService _cmsContentService;
        private readonly ILogger<CmsContentController> _logger;
        private readonly CommonLanguageService _languageService;

        public CmsContentController(
            ICmsContentService cmsContentService,
            ILogger<CmsContentController> logger,
            CommonLanguageService languageService)
        {
            _cmsContentService = cmsContentService;
            _logger = logger;
            _languageService = languageService;
        }

        /// <summary>
        /// User-facing GET: key + language (header/query) se localized content.
        /// Example keys: terms_and_conditions, privacy_policy
        /// Also supports numeric ID: /api/cms/19
        /// </summary>
        [HttpGet("{keyOrId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByKeyOrId(string keyOrId, [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();

                // Check if input is numeric (ID) or string (key)
                if (int.TryParse(keyOrId, out int id))
                {
                    // It's an ID
                    var content = await _cmsContentService.GetByIdAsync(id, currentLanguage);
                    if (content == null)
                    {
                        return NotFound(new
                        {
                            success = false,
                            message = "CMS content not found",
                            id
                        });
                    }

                    return Ok(new
                    {
                        success = true,
                        data = content,
                        language = currentLanguage,
                        message = "CMS content fetched successfully"
                    });
                }
                else
                {
                    // It's a key
                    var content = await _cmsContentService.GetByKeyAsync(keyOrId, currentLanguage);
                    if (content == null)
                    {
                        return NotFound(new
                        {
                            success = false,
                            message = "CMS content not found",
                            key = keyOrId
                        });
                    }

                    return Ok(new
                    {
                        success = true,
                        data = content,
                        language = currentLanguage,
                        message = "CMS content fetched successfully"
                    });
                }
            }
            catch (CmsContentKeyInvalidException ex)
            {
                return BadRequest(new { success = false, message = ex.Message, key = ex.Key });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving CMS content for {keyOrId}", keyOrId);
                return StatusCode(500, new { success = false, message = "Error fetching CMS content" });
            }
        }

        /// <summary>Allowed CMS keys (exact match required for Create/Update).</summary>
        [HttpGet("keys")]
        [AllowAnonymous]
        public IActionResult GetAllowedKeys()
        {
            var keys = _cmsContentService.GetAllowedKeys();
            return Ok(new { success = true, data = keys, message = "Allowed keys (use exactly as listed for Create/Update)." });
        }

        /// <summary>
        /// User-facing: saare active CMS content list (terms, privacy, about, etc.)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var items = await _cmsContentService.GetAllAsync(currentLanguage);

                return Ok(new
                {
                    success = true,
                    data = items,
                    language = currentLanguage,
                    message = "CMS contents fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving CMS contents");
                return StatusCode(500, new { success = false, message = "Error fetching CMS contents" });
            }
        }

        /// <summary>
        /// Admin-facing: saare CMS content (active + inactive) with full translations
        /// </summary>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAdmin()
        {
            try
            {
                var items = await _cmsContentService.GetAllWithTranslationsAsync();

                return Ok(new
                {
                    success = true,
                    data = items,
                    message = "All CMS contents (including inactive) fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all CMS contents for admin");
                return StatusCode(500, new { success = false, message = "Error fetching CMS contents" });
            }
        }

        /// <summary>
        /// Admin create/update karega. EN required, HI optional.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCmsContentDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid request data", errors = ModelState });
                }

                var created = await _cmsContentService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetByKeyOrId), new { keyOrId = created.Key }, new 
                { 
                    success = true, 
                    data = created,
                    message = "CMS content created successfully"
                });
            }
            catch (CmsContentKeyInvalidException ex)
            {
                return BadRequest(new { success = false, message = ex.Message, key = ex.Key });
            }
            catch (CmsContentKeyAlreadyDefinedException ex)
            {
                return Conflict(new { success = false, message = ex.Message, key = ex.Key });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating CMS content");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCmsContentDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid request data", errors = ModelState });
                }

                var updated = await _cmsContentService.UpdateAsync(id, updateDto);
                if (updated == null)
                {
                    return NotFound(new { success = false, message = "CMS content not found" });
                }

                return Ok(new 
                { 
                    success = true, 
                    data = updated,
                    message = "CMS content updated successfully"
                });
            }
            catch (CmsContentKeyInvalidException ex)
            {
                return BadRequest(new { success = false, message = ex.Message, key = ex.Key });
            }
            catch (CmsContentKeyAlreadyDefinedException ex)
            {
                return Conflict(new { success = false, message = ex.Message, key = ex.Key });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating CMS content");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _cmsContentService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = "CMS content not found" });
                }

                return Ok(new { success = true, message = "CMS content deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting CMS content");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>Update status using enum. Body: { "status": "Active" } or { "status": "Inactive" }</summary>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateCmsStatusDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid request data", errors = ModelState });
                }

                var result = await _cmsContentService.UpdateStatusAsync(id, dto.Status);
                if (!result)
                {
                    return NotFound(new { success = false, message = "CMS content not found" });
                }

                return Ok(new { success = true, message = $"CMS content {(dto.Status == CmsContentStatus.Active ? "activated" : "deactivated")} successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating CMS content status");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}

