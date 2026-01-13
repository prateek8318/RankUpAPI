using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.DTOs;
using PaymentService.Application.Services;
using PaymentApplicationService = PaymentService.Application.Services.PaymentService;

namespace PaymentService.API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentApplicationService _service;

        public PaymentController(PaymentApplicationService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll()
        {
            var payments = await _service.GetAllAsync();
            return Ok(payments);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PaymentDto>> GetById(int id)
        {
            var payment = await _service.GetByIdAsync(id);
            if (payment == null)
                return NotFound();
            return Ok(payment);
        }

        [HttpGet("transaction/{transactionId}")]
        [Authorize]
        public async Task<ActionResult<PaymentDto>> GetByTransactionId(string transactionId)
        {
            var payment = await _service.GetByTransactionIdAsync(transactionId);
            if (payment == null)
                return NotFound();
            return Ok(payment);
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByUser(int userId)
        {
            var payments = await _service.GetByUserIdAsync(userId);
            return Ok(payments);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PaymentDto>> Create(CreatePaymentDto dto)
        {
            var payment = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
        }

        [HttpPatch("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdatePaymentStatusDto dto)
        {
            var result = await _service.UpdateStatusAsync(id, dto);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
