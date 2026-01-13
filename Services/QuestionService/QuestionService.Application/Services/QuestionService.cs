using AutoMapper;
using QuestionService.Application.DTOs;
using QuestionService.Application.Interfaces;
using QuestionService.Domain.Entities;

namespace QuestionService.Application.Services
{
    public class QuestionService
    {
        private readonly IQuestionRepository _repository;
        private readonly IMapper _mapper;

        public QuestionService(IQuestionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<QuestionDto> CreateAsync(CreateQuestionDto dto)
        {
            var question = _mapper.Map<Question>(dto);
            question.CreatedAt = DateTime.UtcNow;
            question.IsActive = true;

            await _repository.AddAsync(question);
            await _repository.SaveChangesAsync();

            return _mapper.Map<QuestionDto>(question);
        }

        public async Task<QuestionDto?> GetByIdAsync(int id)
        {
            var question = await _repository.GetByIdAsync(id);
            return question == null ? null : _mapper.Map<QuestionDto>(question);
        }

        public async Task<IEnumerable<QuestionDto>> GetAllAsync()
        {
            var questions = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<QuestionDto>>(questions);
        }

        public async Task<IEnumerable<QuestionDto>> GetByChapterIdAsync(int chapterId)
        {
            var questions = await _repository.GetByChapterIdAsync(chapterId);
            return _mapper.Map<IEnumerable<QuestionDto>>(questions);
        }

        public async Task<QuestionDto?> UpdateAsync(int id, UpdateQuestionDto dto)
        {
            var question = await _repository.GetByIdAsync(id);
            if (question == null)
                return null;

            _mapper.Map(dto, question);
            question.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(question);
            await _repository.SaveChangesAsync();

            return _mapper.Map<QuestionDto>(question);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var question = await _repository.GetByIdAsync(id);
            if (question == null)
                return false;

            await _repository.DeleteAsync(question);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
