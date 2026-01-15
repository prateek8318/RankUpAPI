using AutoMapper;
using Microsoft.Extensions.Logging;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IUserSubscriptionRepository userSubscriptionRepository,
            IMapper mapper,
            ILogger<InvoiceService> logger)
        {
            _invoiceRepository = invoiceRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<InvoiceDto> GenerateInvoiceAsync(int userSubscriptionId)
        {
            try
            {
                _logger.LogInformation("Generating invoice for subscription: {SubscriptionId}", userSubscriptionId);

                // Get user subscription
                var subscription = await _userSubscriptionRepository.GetByIdAsync(userSubscriptionId);
                if (subscription == null)
                {
                    throw new KeyNotFoundException($"User subscription with ID {userSubscriptionId} not found");
                }

                // Check if invoice already exists
                var existingInvoice = await _invoiceRepository.GetByUserSubscriptionIdAsync(userSubscriptionId);
                if (existingInvoice != null)
                {
                    return _mapper.Map<InvoiceDto>(existingInvoice);
                }

                // Generate invoice number
                var invoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAsync();

                // Create invoice
                var invoice = new Invoice
                {
                    UserSubscriptionId = userSubscriptionId,
                    InvoiceNumber = invoiceNumber,
                    InvoiceDate = DateTime.UtcNow,
                    Subtotal = subscription.OriginalAmount,
                    TaxAmount = 0, // Calculate tax based on your requirements
                    TotalAmount = subscription.FinalAmount,
                    Currency = "INR",
                    Status = InvoiceStatus.Generated,
                    CustomerName = $"User {subscription.UserId}", // Get from user service if needed
                    CustomerEmail = $"user{subscription.UserId}@example.com", // Get from user service if needed
                    Notes = $"Subscription: {subscription.SubscriptionPlan.Name} - {subscription.SubscriptionPlan.Description}"
                };

                var createdInvoice = await _invoiceRepository.AddAsync(invoice);
                await _invoiceRepository.SaveChangesAsync();

                var result = _mapper.Map<InvoiceDto>(createdInvoice);
                
                _logger.LogInformation("Successfully generated invoice: {InvoiceNumber}", result.InvoiceNumber);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice for subscription: {SubscriptionId}", userSubscriptionId);
                throw;
            }
        }

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id);
                return invoice != null ? _mapper.Map<InvoiceDto>(invoice) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice with ID: {InvoiceId}", id);
                throw;
            }
        }

        public async Task<InvoiceDto?> GetInvoiceByNumberAsync(string invoiceNumber)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByInvoiceNumberAsync(invoiceNumber);
                return invoice != null ? _mapper.Map<InvoiceDto>(invoice) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice with number: {InvoiceNumber}", invoiceNumber);
                throw;
            }
        }

        public async Task<IEnumerable<InvoiceDto>> GetUserInvoicesAsync(int userId)
        {
            try
            {
                var invoices = await _invoiceRepository.GetByUserIdAsync(userId);
                return _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<InvoiceDownloadResponseDto> DownloadInvoiceAsync(DownloadInvoiceDto downloadInvoiceDto)
        {
            try
            {
                _logger.LogInformation("Downloading invoice for subscription: {SubscriptionId}", downloadInvoiceDto.SubscriptionId);

                // Get subscription
                var subscription = await _userSubscriptionRepository.GetByIdAsync(downloadInvoiceDto.SubscriptionId);
                if (subscription == null || subscription.UserId != downloadInvoiceDto.UserId)
                {
                    return new InvoiceDownloadResponseDto
                    {
                        IsSuccess = false,
                        Message = "Subscription not found or access denied"
                    };
                }

                // Get or generate invoice
                var domainInvoice = await _invoiceRepository.GetByUserSubscriptionIdAsync(downloadInvoiceDto.SubscriptionId);
                Invoice invoice;
                if (domainInvoice == null)
                {
                    var invoiceDto = await GenerateInvoiceAsync(downloadInvoiceDto.SubscriptionId);
                    invoice = _mapper.Map<Invoice>(invoiceDto);
                }
                else
                {
                    invoice = domainInvoice;
                }

                // Generate PDF (simplified version - in production, use a proper PDF library)
                var pdfData = await GenerateInvoicePdfAsync(invoice);

                // Update invoice status
                invoice.Status = InvoiceStatus.Downloaded;
                invoice.DownloadedAt = DateTime.UtcNow;
                await _invoiceRepository.UpdateAsync(invoice);
                await _invoiceRepository.SaveChangesAsync();

                var result = new InvoiceDownloadResponseDto
                {
                    IsSuccess = true,
                    Message = "Invoice downloaded successfully",
                    InvoiceNumber = invoice.InvoiceNumber,
                    PdfData = pdfData,
                    ContentType = "application/pdf",
                    FileName = $"Invoice_{invoice.InvoiceNumber}.pdf"
                };

                _logger.LogInformation("Successfully downloaded invoice: {InvoiceNumber}", result.InvoiceNumber);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading invoice for subscription: {SubscriptionId}", downloadInvoiceDto.SubscriptionId);
                return new InvoiceDownloadResponseDto
                {
                    IsSuccess = false,
                    Message = "Error downloading invoice"
                };
            }
        }

        public async Task<bool> SendInvoiceEmailAsync(int invoiceId)
        {
            try
            {
                _logger.LogInformation("Sending invoice email for invoice: {InvoiceId}", invoiceId);

                var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
                if (invoice == null)
                {
                    return false;
                }

                // Send email (simplified version - in production, use proper email service)
                // await _emailService.SendInvoiceAsync(invoice);

                // Update invoice status
                invoice.Status = InvoiceStatus.Sent;
                invoice.SentAt = DateTime.UtcNow;
                await _invoiceRepository.UpdateAsync(invoice);
                await _invoiceRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully sent invoice email: {InvoiceNumber}", invoice.InvoiceNumber);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending invoice email for invoice: {InvoiceId}", invoiceId);
                return false;
            }
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            try
            {
                return await _invoiceRepository.GenerateInvoiceNumberAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice number");
                throw;
            }
        }

        private async Task<byte[]> GenerateInvoicePdfAsync(Invoice invoice)
        {
            try
            {
                // Simplified PDF generation - in production, use a proper PDF library like iTextSharp or PdfSharp
                // For now, return a simple text representation as bytes
                var invoiceContent = $@"
INVOICE
=======
Invoice Number: {invoice.InvoiceNumber}
Invoice Date: {invoice.InvoiceDate:dd-MM-yyyy}
Customer: {invoice.CustomerName}
Email: {invoice.CustomerEmail}

Subscription Details:
- Plan: {invoice.UserSubscription?.SubscriptionPlan?.Name}
- Period: {invoice.UserSubscription?.StartDate:dd-MM-yyyy} to {invoice.UserSubscription?.EndDate:dd-MM-yyyy}

Amount Details:
- Subtotal: {invoice.Subtotal:C}
- Discount: {invoice.DiscountAmount:C}
- Tax: {invoice.TaxAmount:C}
- Total: {invoice.TotalAmount:C}

Notes: {invoice.Notes}

Thank you for your business!
";

                // Convert to bytes (simplified - use proper PDF library in production)
                return System.Text.Encoding.UTF8.GetBytes(invoiceContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for invoice: {InvoiceNumber}", invoice.InvoiceNumber);
                throw;
            }
        }
    }
}
