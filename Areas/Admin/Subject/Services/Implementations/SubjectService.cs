using AutoMapper;
using RankUpAPI.Areas.Admin.Subject.DTOs;
using RankUpAPI.Areas.Admin.Subject.Services.Interfaces;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;
using SubjectModel = RankUpAPI.Models.Subject;

namespace RankUpAPI.Areas.Admin.Subject.Services.Implementations
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public SubjectService(ISubjectRepository subjectRepository, IMapper mapper)
        {
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createDto)
        {
            var subject = _mapper.Map<SubjectModel>(createDto);
            subject.CreatedAt = DateTime.UtcNow;
            subject.IsActive = true;

            await _subjectRepository.AddAsync(subject);
            await _subjectRepository.SaveChangesAsync();

            return await GetSubjectByIdAsync(subject.Id) ?? throw new Exception("Failed to retrieve created subject");
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
                return false;

            await _subjectRepository.DeleteAsync(subject);
            await _subjectRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync()
        {
            var subjects = await _subjectRepository.GetActiveAsync();

            return subjects.OrderBy(s => s.Name).Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ExamId = s.ExamId,
                ExamName = s.Exam.Name,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                ChapterCount = s.Chapters.Count
            });
        }

        public async Task<SubjectDto?> GetSubjectByIdAsync(int id)
        {
            var subject = await _subjectRepository.GetByIdWithDetailsAsync(id);

            if (subject == null)
                return null;

            return new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name,
                Description = subject.Description,
                ExamId = subject.ExamId,
                ExamName = subject.Exam.Name,
                IsActive = subject.IsActive,
                CreatedAt = subject.CreatedAt,
                UpdatedAt = subject.UpdatedAt,
                ChapterCount = subject.Chapters.Count
            };
        }

        public async Task<IEnumerable<SubjectDto>> GetSubjectsByExamIdAsync(int examId)
        {
            var subjects = await _subjectRepository.GetByExamIdAsync(examId);

            return subjects.OrderBy(s => s.Name).Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ExamId = s.ExamId,
                ExamName = s.Exam.Name,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                ChapterCount = s.Chapters.Count
            });
        }

        public async Task<bool> ToggleSubjectStatusAsync(int id, bool isActive)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
                return false;

            subject.IsActive = isActive;
            subject.UpdatedAt = DateTime.UtcNow;
            await _subjectRepository.UpdateAsync(subject);
            await _subjectRepository.SaveChangesAsync();
            return true;
        }

        public async Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectDto updateDto)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
                return null;

            subject.Name = updateDto.Name;
            subject.Description = updateDto.Description;
            subject.IsActive = updateDto.IsActive;
            subject.UpdatedAt = DateTime.UtcNow;

            await _subjectRepository.UpdateAsync(subject);
            await _subjectRepository.SaveChangesAsync();
            return await GetSubjectByIdAsync(id);
        }
    }
}
