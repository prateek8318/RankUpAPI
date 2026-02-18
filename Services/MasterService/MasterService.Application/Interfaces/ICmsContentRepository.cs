using MasterService.Domain.Entities;

namespace MasterService.Application.Interfaces
{
    public interface ICmsContentRepository
    {
        Task<CmsContent?> GetByIdAsync(int id);
        Task<CmsContent?> GetByKeyAsync(string key);
        Task<bool> ExistsByKeyAsync(string key);
        Task<bool> ExistsByKeyExceptIdAsync(string key, int excludeId);
        Task<IEnumerable<CmsContent>> GetAllAsync();
        Task<IEnumerable<CmsContent>> GetActiveAsync();
        Task<CmsContent> AddAsync(CmsContent content);
        Task UpdateAsync(CmsContent content);
        Task DeleteAsync(CmsContent content);
        Task<int> SaveChangesAsync();
        Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation);
    }
}

