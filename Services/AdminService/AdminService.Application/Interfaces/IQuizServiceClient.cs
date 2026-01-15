namespace AdminService.Application.Interfaces
{
    public interface IQuizServiceClient
    {
        Task<object?> GetAllQuizzesAsync();
        Task<object?> GetQuizByIdAsync(int id);
        Task<object?> CreateQuizAsync(object createDto);
        Task<object?> UpdateQuizAsync(int id, object updateDto);
        Task<bool> DeleteQuizAsync(int id);
    }
}
