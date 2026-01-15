using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;
using AdminService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Text.Json;

namespace AdminService.Application.Services
{
    public class ExportService : IExportService
    {
        private readonly IUserServiceClient _userServiceClient;
        private readonly IExamServiceClient _examServiceClient;
        private readonly ISubscriptionServiceClient _subscriptionServiceClient;
        private readonly IQuizServiceClient _quizServiceClient;
        private readonly IExportLogRepository _exportLogRepository;
        private readonly ILogger<ExportService> _logger;
        private readonly string _exportDirectory;

        public ExportService(
            IUserServiceClient userServiceClient,
            IExamServiceClient examServiceClient,
            ISubscriptionServiceClient subscriptionServiceClient,
            IQuizServiceClient quizServiceClient,
            IExportLogRepository exportLogRepository,
            ILogger<ExportService> logger)
        {
            _userServiceClient = userServiceClient;
            _examServiceClient = examServiceClient;
            _subscriptionServiceClient = subscriptionServiceClient;
            _quizServiceClient = quizServiceClient;
            _exportLogRepository = exportLogRepository;
            _logger = logger;
            _exportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Exports");
            
            if (!Directory.Exists(_exportDirectory))
            {
                Directory.CreateDirectory(_exportDirectory);
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<ExportResultDto> ExportUsersToExcelAsync(ExportFilterDto? filter = null, int adminId = 0)
        {
            var exportLog = new ExportLog
            {
                AdminId = adminId,
                ExportType = "Users",
                Format = "Excel",
                Status = ExportStatus.Processing,
                FilterCriteria = filter?.ToJson(),
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var users = await _userServiceClient.GetAllUsersAsync(1, 10000);
                if (users == null)
                {
                    exportLog.Status = ExportStatus.Failed;
                    exportLog.ErrorMessage = "Failed to retrieve users";
                    await _exportLogRepository.AddAsync(exportLog);
                    await _exportLogRepository.SaveChangesAsync();
                    throw new Exception("Failed to retrieve users");
                }

                var fileName = $"Users_Export_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(_exportDirectory, fileName);

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Users");

                // Headers
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Email";
                worksheet.Cells[1, 3].Value = "Name";
                worksheet.Cells[1, 4].Value = "Created At";
                worksheet.Cells[1, 5].Value = "Is Active";

                // Data
                int row = 2;
                foreach (var user in users)
                {
                    worksheet.Cells[row, 1].Value = user.Id;
                    worksheet.Cells[row, 2].Value = user.Email;
                    worksheet.Cells[row, 3].Value = user.Name;
                    worksheet.Cells[row, 4].Value = user.CreatedAt;
                    worksheet.Cells[row, 5].Value = user.IsActive;
                    row++;
                }

                // Format headers
                worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[1, 1, 1, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                var fileInfo = new FileInfo(filePath);
                await package.SaveAsAsync(fileInfo);

                exportLog.FileName = fileName;
                exportLog.FilePath = filePath;
                exportLog.FileSizeBytes = fileInfo.Length;
                exportLog.RecordCount = users.Count();
                exportLog.Status = ExportStatus.Completed;
                exportLog.CompletedAt = DateTime.UtcNow;

                await _exportLogRepository.AddAsync(exportLog);
                await _exportLogRepository.SaveChangesAsync();

                return new ExportResultDto
                {
                    ExportLogId = exportLog.Id,
                    FileName = fileName,
                    FilePath = filePath,
                    FileSizeBytes = fileInfo.Length,
                    RecordCount = users.Count(),
                    Status = ExportStatus.Completed,
                    CreatedAt = exportLog.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting users to Excel");
                exportLog.Status = ExportStatus.Failed;
                exportLog.ErrorMessage = ex.Message;
                exportLog.CompletedAt = DateTime.UtcNow;
                await _exportLogRepository.AddAsync(exportLog);
                await _exportLogRepository.SaveChangesAsync();
                throw;
            }
        }

        public async Task<ExportResultDto> ExportExamsToExcelAsync(ExportFilterDto? filter = null, int adminId = 0)
        {
            var exportLog = new ExportLog
            {
                AdminId = adminId,
                ExportType = "Exams",
                Format = "Excel",
                Status = ExportStatus.Processing,
                FilterCriteria = filter?.ToJson(),
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var exams = await _examServiceClient.GetAllExamsAsync();
                if (exams == null)
                {
                    exportLog.Status = ExportStatus.Failed;
                    exportLog.ErrorMessage = "Failed to retrieve exams";
                    await _exportLogRepository.AddAsync(exportLog);
                    await _exportLogRepository.SaveChangesAsync();
                    throw new Exception("Failed to retrieve exams");
                }

                var fileName = $"Exams_Export_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(_exportDirectory, fileName);

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Exams");

                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Description";
                worksheet.Cells[1, 4].Value = "Created At";

                int row = 2;
                foreach (var exam in exams)
                {
                    worksheet.Cells[row, 1].Value = exam.Id;
                    worksheet.Cells[row, 2].Value = exam.Name;
                    worksheet.Cells[row, 3].Value = exam.Description;
                    worksheet.Cells[row, 4].Value = exam.CreatedAt;
                    row++;
                }

                worksheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[1, 1, 1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                var fileInfo = new FileInfo(filePath);
                await package.SaveAsAsync(fileInfo);

                exportLog.FileName = fileName;
                exportLog.FilePath = filePath;
                exportLog.FileSizeBytes = fileInfo.Length;
                exportLog.RecordCount = exams.Count();
                exportLog.Status = ExportStatus.Completed;
                exportLog.CompletedAt = DateTime.UtcNow;

                await _exportLogRepository.AddAsync(exportLog);
                await _exportLogRepository.SaveChangesAsync();

                return new ExportResultDto
                {
                    ExportLogId = exportLog.Id,
                    FileName = fileName,
                    FilePath = filePath,
                    FileSizeBytes = fileInfo.Length,
                    RecordCount = exams.Count(),
                    Status = ExportStatus.Completed,
                    CreatedAt = exportLog.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting exams to Excel");
                exportLog.Status = ExportStatus.Failed;
                exportLog.ErrorMessage = ex.Message;
                exportLog.CompletedAt = DateTime.UtcNow;
                await _exportLogRepository.AddAsync(exportLog);
                await _exportLogRepository.SaveChangesAsync();
                throw;
            }
        }

        public async Task<ExportResultDto> ExportSubscriptionsToExcelAsync(ExportFilterDto? filter = null, int adminId = 0)
        {
            // Similar implementation to ExportUsersToExcelAsync
            throw new NotImplementedException("Export subscriptions to Excel not yet implemented");
        }

        public async Task<ExportResultDto> ExportQuizzesToExcelAsync(ExportFilterDto? filter = null, int adminId = 0)
        {
            // Similar implementation to ExportUsersToExcelAsync
            throw new NotImplementedException("Export quizzes to Excel not yet implemented");
        }

        public async Task<ExportLogDto?> GetExportLogByIdAsync(int id)
        {
            try
            {
                var log = await _exportLogRepository.GetByIdAsync(id);
                if (log == null)
                    return null;

                return new ExportLogDto
                {
                    Id = log.Id,
                    AdminId = log.AdminId,
                    AdminEmail = $"Admin_{log.AdminId}", // Admin references UserId, would need UserService call to get email
                    ExportType = log.ExportType,
                    FilePath = log.FilePath,
                    FileName = log.FileName,
                    FileSizeBytes = log.FileSizeBytes,
                    Format = log.Format,
                    RecordCount = log.RecordCount,
                    Status = (ExportStatus)log.Status,
                    ErrorMessage = log.ErrorMessage,
                    CompletedAt = log.CompletedAt,
                    CreatedAt = log.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting export log {id}");
                throw;
            }
        }

        public async Task<IEnumerable<ExportLogDto>> GetExportLogsAsync(int? adminId = null, int page = 1, int pageSize = 50)
        {
            try
            {
                var logs = await _exportLogRepository.GetExportLogsAsync(adminId, page, pageSize);
                return logs.Select(log => new ExportLogDto
                {
                    Id = log.Id,
                    AdminId = log.AdminId,
                    AdminEmail = $"Admin_{log.AdminId}", // Admin references UserId, would need UserService call to get email
                    ExportType = log.ExportType,
                    FilePath = log.FilePath,
                    FileName = log.FileName,
                    FileSizeBytes = log.FileSizeBytes,
                    Format = log.Format,
                    RecordCount = log.RecordCount,
                    Status = (ExportStatus)log.Status,
                    ErrorMessage = log.ErrorMessage,
                    CompletedAt = log.CompletedAt,
                    CreatedAt = log.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting export logs");
                throw;
            }
        }
    }
}
