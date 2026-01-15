using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> GenerateInvoiceAsync(int userSubscriptionId);
        Task<InvoiceDto?> GetInvoiceByIdAsync(int id);
        Task<InvoiceDto?> GetInvoiceByNumberAsync(string invoiceNumber);
        Task<IEnumerable<InvoiceDto>> GetUserInvoicesAsync(int userId);
        Task<InvoiceDownloadResponseDto> DownloadInvoiceAsync(DownloadInvoiceDto downloadInvoiceDto);
        Task<bool> SendInvoiceEmailAsync(int invoiceId);
        Task<string> GenerateInvoiceNumberAsync();
    }
}
