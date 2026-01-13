using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionApplicationService = SubscriptionService.Application.Services.SubscriptionService;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.API.Controllers
{
    [Route("api/subscriptions")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionApplicationService _service;

        public SubscriptionController(SubscriptionApplicationService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetAll()
        {
            var subscriptions = await _service.GetAllAsync();
            return Ok(subscriptions);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SubscriptionDto>> GetById(int id)
        {
            var subscription = await _service.GetByIdAsync(id);
            if (subscription == null)
                return NotFound();
            return Ok(subscription);
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetByUser(int userId)
        {
            var subscriptions = await _service.GetByUserIdAsync(userId);
            return Ok(subscriptions);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SubscriptionDto>> Create(CreateSubscriptionDto dto)
        {
            var subscription = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] SubscriptionStatus status)
        {
            var result = await _service.UpdateStatusAsync(id, status);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
