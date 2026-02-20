using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Services;
using ILanguageService = Common.Services.ILanguageService;

namespace MasterService.API.Controllers
{
    [Route("api/streams")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        private readonly IStreamService _streamService;
        private readonly ILogger<StreamController> _logger;
        private readonly ILanguageService _languageService;

        public StreamController(IStreamService streamService, ILogger<StreamController> logger, ILanguageService languageService)
        {
            _streamService = streamService;
            _logger = logger;
            _languageService = languageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<StreamDto>>> GetStreams([FromQuery] int? languageId = null, [FromQuery] int? qualificationId = null)
        {
            try
            {
                if (!languageId.HasValue)
                    languageId = GetLanguageIdFromHeader();

                IEnumerable<StreamDto> list = qualificationId.HasValue
                    ? await _streamService.GetStreamsByQualificationIdAsync(qualificationId.Value, languageId)
                    : await _streamService.GetAllStreamsAsync(languageId);

                var filtered = FilterByLanguage(list, languageId);
                return Ok(filtered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving streams");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<StreamDto>> GetStream(int id, [FromQuery] int? languageId = null)
        {
            try
            {
                if (!languageId.HasValue)
                    languageId = GetLanguageIdFromHeader();

                var stream = await _streamService.GetStreamByIdAsync(id, languageId);
                if (stream == null)
                    return NotFound();

                var filtered = FilterByLanguage(new[] { stream }, languageId).FirstOrDefault();
                return Ok(filtered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stream {StreamId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StreamDto>> CreateStream(CreateStreamDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var stream = await _streamService.CreateStreamAsync(createDto);
                return CreatedAtAction(nameof(GetStream), new { id = stream.Id }, stream);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating stream");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStream(int id, UpdateStreamDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID in the URL does not match the ID in the request body.");

            try
            {
                var result = await _streamService.UpdateStreamAsync(id, updateDto);
                if (result == null)
                    return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stream {StreamId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStream(int id)
        {
            var result = await _streamService.DeleteStreamAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStreamStatus(int id, [FromBody] bool isActive)
        {
            var result = await _streamService.ToggleStreamStatusAsync(id, isActive);
            if (!result)
                return NotFound();
            return NoContent();
        }

        private int? GetLanguageIdFromHeader()
        {
            var code = _languageService.GetCurrentLanguage();
            return code switch
            {
                "en" => 50,
                "hi" => 49,
                _ => 50
            };
        }

        private static IEnumerable<StreamDto> FilterByLanguage(IEnumerable<StreamDto> list, int? languageId)
        {
            if (!languageId.HasValue)
                return list;
            return list.Select(s => new StreamDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                QualificationId = s.QualificationId,
                QualificationName = s.QualificationName,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                Names = s.Names?.Where(n => n.LanguageId == languageId.Value).ToList() ?? new List<StreamLanguageDto>()
            });
        }
    }
}
