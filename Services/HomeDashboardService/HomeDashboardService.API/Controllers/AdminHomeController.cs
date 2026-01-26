using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Domain.Entities;
using AutoMapper;

namespace HomeDashboardService.API.Controllers
{
    /// <summary>
    /// Admin Home Page Management Controller
    /// </summary>
    [Route("api/admin/home")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminHomeController : ControllerBase
    {
        private readonly IHomeBannerRepository _homeBannerRepository;
        private readonly IOfferBannerRepository _offerBannerRepository;
        private readonly IPracticeModeRepository _practiceModeRepository;
        private readonly IRapidFireTestRepository _rapidFireTestRepository;
        private readonly IFreeTestRepository _freeTestRepository;
        private readonly IMotivationMessageRepository _motivationMessageRepository;
        private readonly ISubscriptionBannerRepository _subscriptionBannerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminHomeController> _logger;
        private readonly IWebHostEnvironment _environment;

        public AdminHomeController(
            IHomeBannerRepository homeBannerRepository,
            IOfferBannerRepository offerBannerRepository,
            IPracticeModeRepository practiceModeRepository,
            IRapidFireTestRepository rapidFireTestRepository,
            IFreeTestRepository freeTestRepository,
            IMotivationMessageRepository motivationMessageRepository,
            ISubscriptionBannerRepository subscriptionBannerRepository,
            IMapper mapper,
            ILogger<AdminHomeController> logger,
            IWebHostEnvironment environment)
        {
            _homeBannerRepository = homeBannerRepository;
            _offerBannerRepository = offerBannerRepository;
            _practiceModeRepository = practiceModeRepository;
            _rapidFireTestRepository = rapidFireTestRepository;
            _freeTestRepository = freeTestRepository;
            _motivationMessageRepository = motivationMessageRepository;
            _subscriptionBannerRepository = subscriptionBannerRepository;
            _mapper = mapper;
            _logger = logger;
            _environment = environment;
        }

        #region Home Banners

        [HttpPost("banners")]
        public async Task<ActionResult<HomeBannerDto>> CreateHomeBanner([FromBody] CreateHomeBannerDto createDto)
        {
            try
            {
                var banner = _mapper.Map<Domain.Entities.HomeBanner>(createDto);
                await _homeBannerRepository.AddAsync(banner);
                await _homeBannerRepository.SaveChangesAsync();
                var result = _mapper.Map<HomeBannerDto>(banner);
                return CreatedAtAction(nameof(GetHomeBanner), new { id = banner.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating home banner");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("banners/{id}")]
        public async Task<ActionResult<HomeBannerDto>> UpdateHomeBanner(int id, [FromBody] UpdateHomeBannerDto updateDto)
        {
            try
            {
                var banner = await _homeBannerRepository.GetByIdAsync(id);
                if (banner == null)
                    return NotFound();

                _mapper.Map(updateDto, banner);
                banner.UpdatedAt = DateTime.UtcNow;
                await _homeBannerRepository.UpdateAsync(banner);
                await _homeBannerRepository.SaveChangesAsync();
                return Ok(_mapper.Map<HomeBannerDto>(banner));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating home banner: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("banners/{id}")]
        public async Task<ActionResult<HomeBannerDto>> GetHomeBanner(int id)
        {
            try
            {
                var banner = await _homeBannerRepository.GetByIdAsync(id);
                if (banner == null)
                    return NotFound();

                return Ok(_mapper.Map<HomeBannerDto>(banner));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting home banner: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("banners")]
        public async Task<ActionResult<List<HomeBannerDto>>> GetAllHomeBanners()
        {
            try
            {
                var banners = await _homeBannerRepository.GetAllAsync();
                return Ok(_mapper.Map<List<HomeBannerDto>>(banners));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all home banners");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("banners/{id}")]
        public async Task<IActionResult> DeleteHomeBanner(int id)
        {
            try
            {
                var result = await _homeBannerRepository.DeleteAsync(id);
                if (!result)
                    return NotFound();

                await _homeBannerRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting home banner: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        #region Offer Banners

        [HttpPost("offer-banners")]
        public async Task<ActionResult<OfferBannerDto>> CreateOfferBanner([FromBody] CreateOfferBannerDto createDto)
        {
            try
            {
                var banner = _mapper.Map<Domain.Entities.OfferBanner>(createDto);
                await _offerBannerRepository.AddAsync(banner);
                await _offerBannerRepository.SaveChangesAsync();
                var result = _mapper.Map<OfferBannerDto>(banner);
                return CreatedAtAction(nameof(GetOfferBanner), new { id = banner.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating offer banner");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("offer-banners/{id}")]
        public async Task<ActionResult<OfferBannerDto>> UpdateOfferBanner(int id, [FromBody] UpdateOfferBannerDto updateDto)
        {
            try
            {
                var banner = await _offerBannerRepository.GetByIdAsync(id);
                if (banner == null)
                    return NotFound();

                _mapper.Map(updateDto, banner);
                banner.UpdatedAt = DateTime.UtcNow;
                await _offerBannerRepository.UpdateAsync(banner);
                await _offerBannerRepository.SaveChangesAsync();
                return Ok(_mapper.Map<OfferBannerDto>(banner));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating offer banner: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("offer-banners/{id}")]
        public async Task<ActionResult<OfferBannerDto>> GetOfferBanner(int id)
        {
            try
            {
                var banner = await _offerBannerRepository.GetByIdAsync(id);
                if (banner == null)
                    return NotFound();

                return Ok(_mapper.Map<OfferBannerDto>(banner));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting offer banner: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("offer-banners")]
        public async Task<ActionResult<List<OfferBannerDto>>> GetAllOfferBanners()
        {
            try
            {
                var banners = await _offerBannerRepository.GetAllAsync();
                return Ok(_mapper.Map<List<OfferBannerDto>>(banners));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all offer banners");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("offer-banners/{id}")]
        public async Task<IActionResult> DeleteOfferBanner(int id)
        {
            try
            {
                var result = await _offerBannerRepository.DeleteAsync(id);
                if (!result)
                    return NotFound();

                await _offerBannerRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting offer banner: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        #region Practice Modes

        [HttpPost("practice-modes")]
        public async Task<ActionResult<PracticeModeDto>> CreatePracticeMode([FromBody] CreatePracticeModeDto createDto)
        {
            try
            {
                var mode = _mapper.Map<Domain.Entities.PracticeMode>(createDto);
                await _practiceModeRepository.AddAsync(mode);
                await _practiceModeRepository.SaveChangesAsync();
                var result = _mapper.Map<PracticeModeDto>(mode);
                return CreatedAtAction(nameof(GetPracticeMode), new { id = mode.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating practice mode");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("practice-modes/{id}")]
        public async Task<ActionResult<PracticeModeDto>> UpdatePracticeMode(int id, [FromBody] UpdatePracticeModeDto updateDto)
        {
            try
            {
                var mode = await _practiceModeRepository.GetByIdAsync(id);
                if (mode == null)
                    return NotFound();

                _mapper.Map(updateDto, mode);
                mode.UpdatedAt = DateTime.UtcNow;
                await _practiceModeRepository.UpdateAsync(mode);
                await _practiceModeRepository.SaveChangesAsync();
                return Ok(_mapper.Map<PracticeModeDto>(mode));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating practice mode: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("practice-modes/{id}")]
        public async Task<ActionResult<PracticeModeDto>> GetPracticeMode(int id)
        {
            try
            {
                var mode = await _practiceModeRepository.GetByIdAsync(id);
                if (mode == null)
                    return NotFound();

                return Ok(_mapper.Map<PracticeModeDto>(mode));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting practice mode: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("practice-modes")]
        public async Task<ActionResult<List<PracticeModeDto>>> GetAllPracticeModes()
        {
            try
            {
                var modes = await _practiceModeRepository.GetAllAsync();
                return Ok(_mapper.Map<List<PracticeModeDto>>(modes));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all practice modes");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("practice-modes/{id}")]
        public async Task<IActionResult> DeletePracticeMode(int id)
        {
            try
            {
                var result = await _practiceModeRepository.DeleteAsync(id);
                if (!result)
                    return NotFound();

                await _practiceModeRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting practice mode: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("practice-modes/{id}/upload-icon")]
        public async Task<IActionResult> UploadPracticeModeIcon(int id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var practiceMode = await _practiceModeRepository.GetByIdAsync(id);
                if (practiceMode == null)
                    return NotFound();

                var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "images", "practice-modes");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_icon_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var iconUrl = $"/images/practice-modes/{uniqueFileName}";
                practiceMode.IconUrl = iconUrl;
                practiceMode.UpdatedAt = DateTime.UtcNow;

                await _practiceModeRepository.UpdateAsync(practiceMode);
                await _practiceModeRepository.SaveChangesAsync();

                return Ok(new { IconUrl = iconUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading icon for practice mode: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        #region Rapid Fire Tests

        [HttpPost("rapid-fire-tests")]
        public async Task<ActionResult<RapidFireTestDto>> CreateRapidFireTest([FromBody] CreateRapidFireTestDto createDto)
        {
            try
            {
                var test = _mapper.Map<Domain.Entities.RapidFireTest>(createDto);
                await _rapidFireTestRepository.AddAsync(test);
                await _rapidFireTestRepository.SaveChangesAsync();
                var result = _mapper.Map<RapidFireTestDto>(test);
                return CreatedAtAction(nameof(GetRapidFireTest), new { id = test.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rapid fire test");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("rapid-fire-tests/{id}")]
        public async Task<ActionResult<RapidFireTestDto>> UpdateRapidFireTest(int id, [FromBody] UpdateRapidFireTestDto updateDto)
        {
            try
            {
                var test = await _rapidFireTestRepository.GetByIdAsync(id);
                if (test == null)
                    return NotFound();

                _mapper.Map(updateDto, test);
                test.UpdatedAt = DateTime.UtcNow;
                await _rapidFireTestRepository.UpdateAsync(test);
                await _rapidFireTestRepository.SaveChangesAsync();
                return Ok(_mapper.Map<RapidFireTestDto>(test));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating rapid fire test: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("rapid-fire-tests/{id}")]
        public async Task<ActionResult<RapidFireTestDto>> GetRapidFireTest(int id)
        {
            try
            {
                var test = await _rapidFireTestRepository.GetByIdAsync(id);
                if (test == null)
                    return NotFound();

                return Ok(_mapper.Map<RapidFireTestDto>(test));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rapid fire test: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("rapid-fire-tests")]
        public async Task<ActionResult<List<RapidFireTestDto>>> GetAllRapidFireTests()
        {
            try
            {
                var tests = await _rapidFireTestRepository.GetAllAsync();
                return Ok(_mapper.Map<List<RapidFireTestDto>>(tests));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all rapid fire tests");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("rapid-fire-tests/{id}")]
        public async Task<IActionResult> DeleteRapidFireTest(int id)
        {
            try
            {
                var result = await _rapidFireTestRepository.DeleteAsync(id);
                if (!result)
                    return NotFound();

                await _rapidFireTestRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting rapid fire test: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // Create Rapid Fire Test with Form Data (supports logo and background image upload)
        [HttpPost("rapid-fire-tests/with-images")]
        public async Task<ActionResult<RapidFireTestImagesDto>> CreateRapidFireTestWithImages(
            [FromForm] CreateRapidFireTestWithImagesDto createDto,
            IFormFile? logoFile = null,
            IFormFile? backgroundImageFile = null)
        {
            try
            {
                var test = _mapper.Map<Domain.Entities.RapidFireTest>(createDto);

                // Handle logo upload
                if (logoFile != null && logoFile.Length > 0)
                {
                    var logoUrl = await UploadImageFile(logoFile, "rapid-fire-tests", "logo");
                    test.LogoUrl = logoUrl;
                }

                // Handle background image upload
                if (backgroundImageFile != null && backgroundImageFile.Length > 0)
                {
                    var backgroundImageUrl = await UploadImageFile(backgroundImageFile, "rapid-fire-tests", "background");
                    test.BackgroundImageUrl = backgroundImageUrl;
                }

                await _rapidFireTestRepository.AddAsync(test);
                await _rapidFireTestRepository.SaveChangesAsync();
                
                var result = _mapper.Map<RapidFireTestImagesDto>(test);
                return CreatedAtAction(nameof(GetRapidFireTestWithImages), new { id = test.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rapid fire test with images");
                return StatusCode(500, "Internal server error");
            }
        }

        // Update Rapid Fire Test with Form Data (supports logo and background image upload)
        [HttpPut("rapid-fire-tests/{id}/with-images")]
        public async Task<ActionResult<RapidFireTestImagesDto>> UpdateRapidFireTestWithImages(
            int id,
            [FromForm] UpdateRapidFireTestWithImagesDto updateDto,
            IFormFile? logoFile = null,
            IFormFile? backgroundImageFile = null)
        {
            try
            {
                var test = await _rapidFireTestRepository.GetByIdAsync(id);
                if (test == null)
                    return NotFound();

                _mapper.Map(updateDto, test);
                test.UpdatedAt = DateTime.UtcNow;

                // Handle logo upload
                if (logoFile != null && logoFile.Length > 0)
                {
                    var logoUrl = await UploadImageFile(logoFile, "rapid-fire-tests", "logo");
                    test.LogoUrl = logoUrl;
                }

                // Handle background image upload
                if (backgroundImageFile != null && backgroundImageFile.Length > 0)
                {
                    var backgroundImageUrl = await UploadImageFile(backgroundImageFile, "rapid-fire-tests", "background");
                    test.BackgroundImageUrl = backgroundImageUrl;
                }

                await _rapidFireTestRepository.UpdateAsync(test);
                await _rapidFireTestRepository.SaveChangesAsync();
                
                var result = _mapper.Map<RapidFireTestImagesDto>(test);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating rapid fire test with images: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // Get Rapid Fire Test with all image URLs
        [HttpGet("rapid-fire-tests/{id}/with-images")]
        public async Task<ActionResult<RapidFireTestImagesDto>> GetRapidFireTestWithImages(int id)
        {
            try
            {
                var test = await _rapidFireTestRepository.GetByIdAsync(id);
                if (test == null)
                    return NotFound();

                return Ok(_mapper.Map<RapidFireTestImagesDto>(test));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rapid fire test with images: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // Upload logo for specific rapid fire test
        [HttpPost("rapid-fire-tests/{id}/upload-logo")]
        public async Task<ActionResult<string>> UploadRapidFireTestLogo(int id, IFormFile logoFile)
        {
            try
            {
                if (logoFile == null || logoFile.Length == 0)
                    return BadRequest("No logo file uploaded.");

                var test = await _rapidFireTestRepository.GetByIdAsync(id);
                if (test == null)
                    return NotFound();

                var logoUrl = await UploadImageFile(logoFile, "rapid-fire-tests", "logo");
                test.LogoUrl = logoUrl;
                test.UpdatedAt = DateTime.UtcNow;

                await _rapidFireTestRepository.UpdateAsync(test);
                await _rapidFireTestRepository.SaveChangesAsync();

                return Ok(new { LogoUrl = logoUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading logo for rapid fire test: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // Upload background image for specific rapid fire test
        [HttpPost("rapid-fire-tests/{id}/upload-background")]
        public async Task<ActionResult<string>> UploadRapidFireTestBackground(int id, IFormFile backgroundImageFile)
        {
            try
            {
                if (backgroundImageFile == null || backgroundImageFile.Length == 0)
                    return BadRequest("No background image file uploaded.");

                var test = await _rapidFireTestRepository.GetByIdAsync(id);
                if (test == null)
                    return NotFound();

                var backgroundImageUrl = await UploadImageFile(backgroundImageFile, "rapid-fire-tests", "background");
                test.BackgroundImageUrl = backgroundImageUrl;
                test.UpdatedAt = DateTime.UtcNow;

                await _rapidFireTestRepository.UpdateAsync(test);
                await _rapidFireTestRepository.SaveChangesAsync();

                return Ok(new { BackgroundImageUrl = backgroundImageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading background image for rapid fire test: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // Helper method for image upload
        private async Task<string> UploadImageFile(IFormFile file, string folderName, string prefix)
        {
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "images", folderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{prefix}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/{folderName}/{uniqueFileName}";
        }

        #endregion

        #region Free Tests

        [HttpPost("free-tests")]
        public async Task<ActionResult<FreeTestDto>> CreateFreeTest([FromBody] CreateFreeTestDto createDto)
        {
            try
            {
                var test = _mapper.Map<Domain.Entities.FreeTest>(createDto);
                await _freeTestRepository.AddAsync(test);
                await _freeTestRepository.SaveChangesAsync();
                var result = _mapper.Map<FreeTestDto>(test);
                return CreatedAtAction(nameof(GetFreeTest), new { id = test.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating free test");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("free-tests/{id}")]
        public async Task<ActionResult<FreeTestDto>> UpdateFreeTest(int id, [FromBody] UpdateFreeTestDto updateDto)
        {
            try
            {
                var test = await _freeTestRepository.GetByIdAsync(id);
                if (test == null)
                    return NotFound();

                _mapper.Map(updateDto, test);
                test.UpdatedAt = DateTime.UtcNow;
                await _freeTestRepository.UpdateAsync(test);
                await _freeTestRepository.SaveChangesAsync();
                return Ok(_mapper.Map<FreeTestDto>(test));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating free test: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("free-tests/{id}")]
        public async Task<ActionResult<FreeTestDto>> GetFreeTest(int id)
        {
            try
            {
                var test = await _freeTestRepository.GetByIdAsync(id);
                if (test == null)
                    return NotFound();

                return Ok(_mapper.Map<FreeTestDto>(test));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting free test: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("free-tests")]
        public async Task<ActionResult<List<FreeTestDto>>> GetAllFreeTests()
        {
            try
            {
                var tests = await _freeTestRepository.GetAllAsync();
                return Ok(_mapper.Map<List<FreeTestDto>>(tests));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all free tests");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("free-tests/{id}")]
        public async Task<IActionResult> DeleteFreeTest(int id)
        {
            try
            {
                var result = await _freeTestRepository.DeleteAsync(id);
                if (!result)
                    return NotFound();

                await _freeTestRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting free test: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        #region Motivation Messages

        [HttpPost("motivation-messages")]
        public async Task<ActionResult> CreateMotivationMessage([FromBody] CreateMotivationMessageDto createDto)
        {
            try
            {
                var message = _mapper.Map<Domain.Entities.MotivationMessage>(createDto);
                await _motivationMessageRepository.AddAsync(message);
                await _motivationMessageRepository.SaveChangesAsync();
                return CreatedAtAction(nameof(GetMotivationMessage), new { id = message.Id }, new { id = message.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating motivation message");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("motivation-messages/{id}")]
        public async Task<IActionResult> UpdateMotivationMessage(int id, [FromBody] UpdateMotivationMessageDto updateDto)
        {
            try
            {
                var message = await _motivationMessageRepository.GetByIdAsync(id);
                if (message == null)
                    return NotFound();

                _mapper.Map(updateDto, message);
                message.UpdatedAt = DateTime.UtcNow;
                await _motivationMessageRepository.UpdateAsync(message);
                await _motivationMessageRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating motivation message: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("motivation-messages/{id}")]
        public async Task<ActionResult> GetMotivationMessage(int id)
        {
            try
            {
                var message = await _motivationMessageRepository.GetByIdAsync(id);
                if (message == null)
                    return NotFound();

                return Ok(new { 
                    id = message.Id,
                    message = message.Message,
                    author = message.Author,
                    type = message.Type,
                    isGreeting = message.IsGreeting,
                    isActive = message.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting motivation message: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("motivation-messages")]
        public async Task<ActionResult> GetAllMotivationMessages()
        {
            try
            {
                var messages = await _motivationMessageRepository.GetAllAsync();
                return Ok(messages.Select(m => new {
                    id = m.Id,
                    message = m.Message,
                    author = m.Author,
                    type = m.Type,
                    isGreeting = m.IsGreeting,
                    isActive = m.IsActive
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all motivation messages");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("motivation-messages/{id}")]
        public async Task<IActionResult> DeleteMotivationMessage(int id)
        {
            try
            {
                var result = await _motivationMessageRepository.DeleteAsync(id);
                if (!result)
                    return NotFound();

                await _motivationMessageRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting motivation message: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        #region Subscription Banners

        [HttpPost("subscription-banners")]
        public async Task<ActionResult<SubscriptionBannerConfigDto>> CreateSubscriptionBanner([FromBody] CreateSubscriptionBannerDto createDto)
        {
            try
            {
                var banner = _mapper.Map<Domain.Entities.SubscriptionBanner>(createDto);
                await _subscriptionBannerRepository.AddAsync(banner);
                await _subscriptionBannerRepository.SaveChangesAsync();
                var result = _mapper.Map<SubscriptionBannerConfigDto>(banner);
                return CreatedAtAction(nameof(GetSubscriptionBanner), new { id = banner.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription banner");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("subscription-banners/{id}")]
        public async Task<ActionResult<SubscriptionBannerConfigDto>> UpdateSubscriptionBanner(int id, [FromBody] UpdateSubscriptionBannerDto updateDto)
        {
            try
            {
                var banner = await _subscriptionBannerRepository.GetByIdAsync(id);
                if (banner == null)
                    return NotFound();

                _mapper.Map(updateDto, banner);
                banner.UpdatedAt = DateTime.UtcNow;
                await _subscriptionBannerRepository.UpdateAsync(banner);
                await _subscriptionBannerRepository.SaveChangesAsync();
                return Ok(_mapper.Map<SubscriptionBannerConfigDto>(banner));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription banner: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("subscription-banners/{id}")]
        public async Task<ActionResult<SubscriptionBannerConfigDto>> GetSubscriptionBanner(int id)
        {
            try
            {
                var banner = await _subscriptionBannerRepository.GetByIdAsync(id);
                if (banner == null)
                    return NotFound();

                return Ok(_mapper.Map<SubscriptionBannerConfigDto>(banner));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription banner: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("subscription-banners")]
        public async Task<ActionResult<List<SubscriptionBannerConfigDto>>> GetAllSubscriptionBanners()
        {
            try
            {
                var banners = await _subscriptionBannerRepository.GetAllAsync();
                return Ok(_mapper.Map<List<SubscriptionBannerConfigDto>>(banners));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subscription banners");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("subscription-banners/{id}")]
        public async Task<IActionResult> DeleteSubscriptionBanner(int id)
        {
            try
            {
                var result = await _subscriptionBannerRepository.DeleteAsync(id);
                if (!result)
                    return NotFound();

                await _subscriptionBannerRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription banner: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion
    }
}
