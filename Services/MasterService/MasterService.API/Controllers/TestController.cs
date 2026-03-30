using Microsoft.AspNetCore.Mvc;
using MasterService.Application.Interfaces;
using Common.DTOs;

namespace MasterService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IExamRepository _examRepository;
        private readonly ICategoryRepository _categoryRepository;

        public TestController(IExamRepository examRepository, ICategoryRepository categoryRepository)
        {
            _examRepository = examRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet("exams")]
        public async Task<IActionResult> TestExams()
        {
            try
            {
                // Test existing functionality (backward compatibility)
                var allExams = await _examRepository.GetAllAsync();
                var activeExams = await _examRepository.GetActiveAsync();

                // Test new pagination functionality
                var paginatedExams = await _examRepository.GetAllAsync(new PaginationRequest { PageNumber = 1, PageSize = 10 });
                var paginatedActiveExams = await _examRepository.GetActiveAsync(new PaginationRequest { PageNumber = 1, PageSize = 10 });

                return Ok(new
                {
                    AllExamsCount = allExams.Count(),
                    ActiveExamsCount = activeExams.Count(),
                    PaginatedExamsCount = paginatedExams.Data.Count(),
                    PaginatedActiveExamsCount = paginatedActiveExams.Data.Count(),
                    PaginatedExamsTotal = paginatedExams.TotalCount,
                    PaginatedActiveTotal = paginatedActiveExams.TotalCount,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        [HttpGet("categories")]
        public async Task<IActionResult> TestCategories()
        {
            try
            {
                // Test existing functionality (backward compatibility)
                var allCategories = await _categoryRepository.GetAllAsync();
                var activeCategories = await _categoryRepository.GetActiveAsync();

                // Test new pagination functionality
                var paginatedCategories = await _categoryRepository.GetAllAsync(new PaginationRequest { PageNumber = 1, PageSize = 10 });
                var paginatedActiveCategories = await _categoryRepository.GetActiveAsync(new PaginationRequest { PageNumber = 1, PageSize = 10 });

                return Ok(new
                {
                    AllCategoriesCount = allCategories.Count(),
                    ActiveCategoriesCount = activeCategories.Count(),
                    PaginatedCategoriesCount = paginatedCategories.Data.Count(),
                    PaginatedActiveCategoriesCount = paginatedActiveCategories.Data.Count(),
                    PaginatedCategoriesTotal = paginatedCategories.TotalCount,
                    PaginatedActiveTotal = paginatedActiveCategories.TotalCount,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }
    }
}
