using AutoMapper;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.Services
{
    public class SubscriptionService
    {
        private readonly ISubscriptionRepository _repository;
        private readonly IMapper _mapper;

        public SubscriptionService(ISubscriptionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto)
        {
            var subscription = _mapper.Map<Subscription>(dto);
            subscription.FinalAmount = dto.Amount - (dto.DiscountAmount ?? 0);
            subscription.Status = SubscriptionStatus.Pending;
            subscription.CreatedAt = DateTime.UtcNow;
            subscription.IsActive = true;

            await _repository.AddAsync(subscription);
            await _repository.SaveChangesAsync();

            return _mapper.Map<SubscriptionDto>(subscription);
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
    }
}
