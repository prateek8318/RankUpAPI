using AutoMapper;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using StreamEntity = MasterService.Domain.Entities.Stream;

namespace MasterService.Application.Services
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
            var stream = _mapper.Map<StreamEntity>(createDto);
            stream.CreatedAt = DateTime.UtcNow;
            stream.IsActive = true;

            if (createDto.Names != null && createDto.Names.Any())
            {
                foreach (var langDto in createDto.Names)
                {
                    stream.StreamLanguages.Add(new StreamLanguage
                    {
                        LanguageId = langDto.LanguageId,
                        Name = langDto.Name,
                        Description = langDto.Description,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            await _streamRepository.AddAsync(stream);
            await _streamRepository.SaveChangesAsync();
            return (await GetStreamByIdAsync(stream.Id))!;
        }

        public async Task<StreamDto?> UpdateStreamAsync(int id, UpdateStreamDto updateDto)
        {
            var stream = await _streamRepository.GetByIdAsync(id);
            if (stream == null)
                return null;

            stream.Name = updateDto.Name;
            stream.Description = updateDto.Description;
            stream.QualificationId = updateDto.QualificationId;
            stream.UpdatedAt = DateTime.UtcNow;

            if (updateDto.Names != null && updateDto.Names.Any())
            {
                var existingLanguages = stream.StreamLanguages.ToList();
                foreach (var existingLang in existingLanguages)
                {
                    stream.StreamLanguages.Remove(existingLang);
                }
                foreach (var langDto in updateDto.Names)
                {
                    stream.StreamLanguages.Add(new StreamLanguage
                    {
                        LanguageId = langDto.LanguageId,
                        Name = langDto.Name,
                        Description = langDto.Description,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            await _streamRepository.UpdateAsync(stream);
            await _streamRepository.SaveChangesAsync();
            return await GetStreamByIdAsync(stream.Id);
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

        public async Task<StreamDto?> GetStreamByIdAsync(int id, int? languageId = null)
        {
            var stream = await _streamRepository.GetByIdAsync(id);
            if (stream == null)
                return null;

            var dto = _mapper.Map<StreamDto>(stream);
            dto.QualificationName = stream.Qualification?.Name;
            if (languageId.HasValue)
            {
                var langName = stream.StreamLanguages.FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Name;
                if (!string.IsNullOrEmpty(langName))
                    dto.Name = langName;
                var langDesc = stream.StreamLanguages.FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Description;
                if (langDesc != null)
                    dto.Description = langDesc;
            }
            return dto;
        }

        public async Task<IEnumerable<StreamDto>> GetAllStreamsAsync(int? languageId = null)
        {
            var streams = await _streamRepository.GetActiveAsync();
            var dtos = streams.OrderBy(s => s.Name).Select(s =>
            {
                var dto = _mapper.Map<StreamDto>(s);
                dto.QualificationName = s.Qualification?.Name;
                return dto;
            }).ToList();
            if (languageId.HasValue)
            {
                foreach (var dto in dtos)
                {
                    var s = streams.FirstOrDefault(x => x.Id == dto.Id);
                    if (s != null)
                    {
                        var langName = s.StreamLanguages.FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName))
                            dto.Name = langName;
                        var langDesc = s.StreamLanguages.FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Description;
                        if (langDesc != null)
                            dto.Description = langDesc;
                    }
                }
            }
            return dtos;
        }

        public async Task<IEnumerable<StreamDto>> GetStreamsByQualificationIdAsync(int qualificationId, int? languageId = null)
        {
            var streams = await _streamRepository.GetActiveByQualificationIdAsync(qualificationId);
            var dtos = streams.OrderBy(s => s.Name).Select(s =>
            {
                var dto = _mapper.Map<StreamDto>(s);
                dto.QualificationName = s.Qualification?.Name;
                return dto;
            }).ToList();
            if (languageId.HasValue)
            {
                foreach (var dto in dtos)
                {
                    var s = streams.FirstOrDefault(x => x.Id == dto.Id);
                    if (s != null)
                    {
                        var langName = s.StreamLanguages.FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName))
                            dto.Name = langName;
                        var langDesc = s.StreamLanguages.FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Description;
                        if (langDesc != null)
                            dto.Description = langDesc;
                    }
                }
            }
            return dtos;
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
