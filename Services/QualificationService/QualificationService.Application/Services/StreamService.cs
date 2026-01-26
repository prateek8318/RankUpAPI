using AutoMapper;
using QualificationService.Application.DTOs;
using QualificationService.Application.Interfaces;
using QualificationService.Domain.Entities;

namespace QualificationService.Application.Services
{
    public class StreamService : IStreamService
    {
        private readonly IStreamRepository _streamRepository;
        private readonly IMapper _mapper;

        public StreamService(IStreamRepository streamRepository, IMapper mapper)
        {
            _streamRepository = streamRepository;
            _mapper = mapper;
        }

        public async Task<StreamDto> CreateStreamAsync(CreateStreamDto createDto)
        {
            var stream = _mapper.Map<Domain.Entities.Stream>(createDto);
            stream.CreatedAt = DateTime.UtcNow;
            stream.IsActive = true;

            await _streamRepository.AddAsync(stream);
            await _streamRepository.SaveChangesAsync();

            return _mapper.Map<StreamDto>(stream);
        }

        public async Task<StreamDto?> UpdateStreamAsync(int id, UpdateStreamDto updateDto)
        {
            var stream = await _streamRepository.GetByIdAsync(id);
            if (stream == null)
                return null;

            _mapper.Map(updateDto, stream);
            stream.UpdatedAt = DateTime.UtcNow;
            await _streamRepository.UpdateAsync(stream);
            await _streamRepository.SaveChangesAsync();

            return _mapper.Map<StreamDto>(stream);
        }

        public async Task<bool> DeleteStreamAsync(int id)
        {
            var stream = await _streamRepository.GetByIdAsync(id);
            if (stream == null)
                return false;

            await _streamRepository.DeleteAsync(stream);
            await _streamRepository.SaveChangesAsync();
            return true;
        }

        public async Task<StreamDto?> GetStreamByIdAsync(int id)
        {
            var stream = await _streamRepository.GetByIdAsync(id);
            return stream == null ? null : _mapper.Map<StreamDto>(stream);
        }

        public async Task<IEnumerable<StreamDto>> GetAllStreamsAsync()
        {
            var streams = await _streamRepository.GetActiveAsync();
            return streams.OrderBy(s => s.Name).Select(s => _mapper.Map<StreamDto>(s));
        }

        public async Task<bool> ToggleStreamStatusAsync(int id, bool isActive)
        {
            var stream = await _streamRepository.GetByIdAsync(id);
            if (stream == null)
                return false;

            stream.IsActive = isActive;
            stream.UpdatedAt = DateTime.UtcNow;
            await _streamRepository.UpdateAsync(stream);
            await _streamRepository.SaveChangesAsync();
            return true;
        }
    }
}
