using AutoMapper;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;

namespace MasterService.Application.Services
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;
        private readonly IMapper _mapper;

        public StateService(IStateRepository stateRepository, IMapper mapper)
        {
            _stateRepository = stateRepository;
            _mapper = mapper;
        }

        public async Task<StateDto> CreateStateAsync(CreateStateDto createDto)
        {
            var state = _mapper.Map<State>(createDto);
            state.CreatedAt = DateTime.UtcNow;
            state.IsActive = true;

            await _stateRepository.AddAsync(state);
            await _stateRepository.SaveChangesAsync();

            return _mapper.Map<StateDto>(state);
        }

        public async Task<StateDto?> UpdateStateAsync(int id, UpdateStateDto updateDto)
        {
            var state = await _stateRepository.GetByIdAsync(id);
            if (state == null)
                return null;

            _mapper.Map(updateDto, state);
            state.UpdatedAt = DateTime.UtcNow;
            await _stateRepository.UpdateAsync(state);
            await _stateRepository.SaveChangesAsync();

            return _mapper.Map<StateDto>(state);
        }

        public async Task<bool> DeleteStateAsync(int id)
        {
            var state = await _stateRepository.GetByIdAsync(id);
            if (state == null)
                return false;

            await _stateRepository.DeleteAsync(state);
            await _stateRepository.SaveChangesAsync();
            return true;
        }

        public async Task<StateDto?> GetStateByIdAsync(int id)
        {
            var state = await _stateRepository.GetByIdAsync(id);
            return state == null ? null : _mapper.Map<StateDto>(state);
        }

        public async Task<IEnumerable<StateDto>> GetAllStatesAsync()
        {
            var states = await _stateRepository.GetActiveAsync();
            return states.OrderBy(s => s.Name).Select(s => _mapper.Map<StateDto>(s));
        }

        public async Task<bool> ToggleStateStatusAsync(int id, bool isActive)
        {
            var state = await _stateRepository.GetByIdAsync(id);
            if (state == null)
                return false;

            state.IsActive = isActive;
            state.UpdatedAt = DateTime.UtcNow;
            await _stateRepository.UpdateAsync(state);
            await _stateRepository.SaveChangesAsync();
            return true;
        }
    }
}
