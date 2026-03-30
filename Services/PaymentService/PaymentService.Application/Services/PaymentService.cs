using AutoMapper;
using Microsoft.Extensions.Logging;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using Common.DTOs;

namespace PaymentService.Application.Services
{
    public class PaymentService : BaseService
    {
        private readonly IPaymentRepository _repository;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository repository, IMapper mapper, ILogger<PaymentService> logger) : base(logger)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto)
        {
            return await ExecuteInTransactionAsync(
                async () =>
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
                },
                "CreatePayment");
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

        // Pagination support methods
        public async Task<PaginatedResponse<PaymentDto>> GetAllPaymentsPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                var paginatedPayments = await _repository.GetAllAsync(pagination);
                var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(paginatedPayments.Data);
                
                return new PaginatedResponse<PaymentDto>
                {
                    Data = paymentDtos,
                    TotalCount = paginatedPayments.TotalCount,
                    PageNumber = paginatedPayments.PageNumber,
                    PageSize = paginatedPayments.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated payments");
                throw;
            }
        }

        public async Task<PaginatedResponse<PaymentDto>> GetPaymentsByUserIdPaginatedAsync(int userId, PaginationRequest pagination)
        {
            try
            {
                var paginatedPayments = await _repository.GetByUserIdAsync(userId, pagination);
                var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(paginatedPayments.Data);
                
                return new PaginatedResponse<PaymentDto>
                {
                    Data = paymentDtos,
                    TotalCount = paginatedPayments.TotalCount,
                    PageNumber = paginatedPayments.PageNumber,
                    PageSize = paginatedPayments.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated payments for user {UserId}", userId);
                throw;
            }
        }
    }
}
