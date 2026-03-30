using AutoMapper;
using Microsoft.Extensions.Logging;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;
using Common.DTOs;

namespace SubscriptionService.Application.Services
{
    public class SubscriptionService : BaseService
    {
        private readonly ISubscriptionRepository _repository;
        private readonly IMapper _mapper;

        public SubscriptionService(ISubscriptionRepository repository, IMapper mapper, ILogger<SubscriptionService> logger) : base(logger)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto)
        {
            return await ExecuteInTransactionAsync(
                async () =>
                {
                    var subscription = _mapper.Map<Subscription>(dto);
                    subscription.FinalAmount = dto.Amount - (dto.DiscountAmount ?? 0);
                    subscription.Status = SubscriptionStatus.Pending;
                    subscription.CreatedAt = DateTime.UtcNow;
                    subscription.IsActive = true;

                    await _repository.AddAsync(subscription);
                    await _repository.SaveChangesAsync();

                    return _mapper.Map<SubscriptionDto>(subscription);
                },
                "CreateSubscription");
        }

        public async Task<SubscriptionDto?> GetByIdAsync(int id)
        {
            var subscription = await _repository.GetByIdAsync(id);
            return subscription == null ? null : _mapper.Map<SubscriptionDto>(subscription);
        }

        public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
        {
            var subscriptions = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<SubscriptionDto>>(subscriptions);
        }

        public async Task<IEnumerable<SubscriptionDto>> GetByUserIdAsync(int userId)
        {
            var subscriptions = await _repository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<SubscriptionDto>>(subscriptions);
        }

        public async Task<SubscriptionDto?> UpdateStatusAsync(int id, SubscriptionStatus status)
        {
            var subscription = await _repository.GetByIdAsync(id);
            if (subscription == null)
                return null;

            subscription.Status = status;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(subscription);
            await _repository.SaveChangesAsync();

            return _mapper.Map<SubscriptionDto>(subscription);
        }

        // Pagination support methods
        public async Task<PaginatedResponse<SubscriptionDto>> GetAllSubscriptionsPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                var paginatedSubscriptions = await _repository.GetAllAsync(pagination);
                var subscriptionDtos = _mapper.Map<IEnumerable<SubscriptionDto>>(paginatedSubscriptions.Data);
                
                return new PaginatedResponse<SubscriptionDto>
                {
                    Data = subscriptionDtos,
                    TotalCount = paginatedSubscriptions.TotalCount,
                    PageNumber = paginatedSubscriptions.PageNumber,
                    PageSize = paginatedSubscriptions.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated subscriptions");
                throw;
            }
        }

        public async Task<PaginatedResponse<SubscriptionDto>> GetSubscriptionsByUserIdPaginatedAsync(int userId, PaginationRequest pagination)
        {
            try
            {
                var paginatedSubscriptions = await _repository.GetByUserIdAsync(userId, pagination);
                var subscriptionDtos = _mapper.Map<IEnumerable<SubscriptionDto>>(paginatedSubscriptions.Data);
                
                return new PaginatedResponse<SubscriptionDto>
                {
                    Data = subscriptionDtos,
                    TotalCount = paginatedSubscriptions.TotalCount,
                    PageNumber = paginatedSubscriptions.PageNumber,
                    PageSize = paginatedSubscriptions.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated subscriptions for user {UserId}", userId);
                throw;
            }
        }
    }
}
