using QualificationService.Application.DTOs;
using QualificationService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QualificationService.API.Controllers
{
    [Route("api/streams")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        private readonly IStreamService _streamService;
        private readonly ILogger<StreamController> _logger;

        public StreamController(IStreamService streamService, ILogger<StreamController> logger)
        {
            _streamService = streamService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<StreamDto>>> GetStreams()
        {
            try
            {
                var streams = await _streamService.GetAllStreamsAsync();
                return Ok(streams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving streams");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<StreamDto>> GetStream(int id)
        {
            try
            {
                var stream = await _streamService.GetStreamByIdAsync(id);
                if (stream == null)
                    return NotFound();

                return Ok(stream);
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

            var result = await _streamService.UpdateStreamAsync(id, updateDto);
            if (result == null)
                return NotFound();

            return NoContent();
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
    }
}
