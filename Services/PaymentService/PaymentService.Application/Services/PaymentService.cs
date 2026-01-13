using AutoMapper;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.Services
{
    public class PaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto)
        {
            var payment = _mapper.Map<Payment>(dto);
            payment.TransactionId = Guid.NewGuid().ToString("N");
            payment.FinalAmount = dto.Amount - (dto.DiscountAmount ?? 0);
            payment.Status = PaymentStatus.Pending;
            payment.CreatedAt = DateTime.UtcNow;
            payment.IsActive = true;

            await _repository.AddAsync(payment);
            await _repository.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto?> GetByIdAsync(int id)
        {
            var payment = await _repository.GetByIdAsync(id);
            return payment == null ? null : _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto?> GetByTransactionIdAsync(string transactionId)
        {
            var payment = await _repository.GetByTransactionIdAsync(transactionId);
            return payment == null ? null : _mapper.Map<PaymentDto>(payment);
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<IEnumerable<PaymentDto>> GetByUserIdAsync(int userId)
        {
            var payments = await _repository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDto?> UpdateStatusAsync(int id, UpdatePaymentStatusDto dto)
        {
            var payment = await _repository.GetByIdAsync(id);
            if (payment == null)
                return null;

            payment.Status = dto.Status;
            payment.RazorpayOrderId = dto.RazorpayOrderId;
            payment.RazorpayPaymentId = dto.RazorpayPaymentId;
            payment.FailureReason = dto.FailureReason;
            
            if (dto.Status == PaymentStatus.Success)
            {
                payment.PaidAt = DateTime.UtcNow;
            }

            payment.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(payment);
            await _repository.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
        }
    }
}
