using ExamService.Domain.Entities;
using ExamService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ExamService.API.Services
{
    public class ExamDataSeedService
    {
        private readonly ExamDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ExamDataSeedService(
            ExamDbContext context,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task SeedExamsAsync(bool clearExisting = false)
        {
            if (clearExisting)
            {
                // Clear existing data
                _context.ExamQualifications.RemoveRange(await _context.ExamQualifications.ToListAsync());
                _context.Exams.RemoveRange(await _context.Exams.ToListAsync());
                await _context.SaveChangesAsync();
            }

            // Get Qualifications from QualificationService
            var qualifications = await GetQualificationsFromServiceAsync();
            if (qualifications == null || !qualifications.Any())
            {
                throw new Exception("Could not fetch qualifications from QualificationService. Please seed qualifications first.");
            }

            // Get Stream IDs from QualificationService
            var streams = await GetStreamsFromServiceAsync();
            if (streams == null || !streams.Any())
            {
                throw new Exception("Could not fetch streams from QualificationService. Please seed streams first.");
            }

            var scienceStream = streams.FirstOrDefault(s => s.Name == "Science");
            var commerceStream = streams.FirstOrDefault(s => s.Name == "Commerce");
            var artsStream = streams.FirstOrDefault(s => s.Name == "Arts");
            var generalStream = streams.FirstOrDefault(s => s.Name == "General");

            // Get Qualification IDs
            var qual10th = qualifications.FirstOrDefault(q => q.Name == "10th Grade");
            var qual12thScience = qualifications.FirstOrDefault(q => q.Name == "12th Grade - Science");
            var qual12thCommerce = qualifications.FirstOrDefault(q => q.Name == "12th Grade - Commerce");
            var qual12thArts = qualifications.FirstOrDefault(q => q.Name == "12th Grade - Arts");
            var qualGradScience = qualifications.FirstOrDefault(q => q.Name == "Graduation - Science");
            var qualGradCommerce = qualifications.FirstOrDefault(q => q.Name == "Graduation - Commerce");
            var qualGradArts = qualifications.FirstOrDefault(q => q.Name == "Graduation - Arts");
            var qualGradGeneral = qualifications.FirstOrDefault(q => q.Name == "Graduation - General");
            var qualPGScience = qualifications.FirstOrDefault(q => q.Name == "Post Graduation - Science");
            var qualPGGeneral = qualifications.FirstOrDefault(q => q.Name == "Post Graduation - General");

            // Seed National Exams
            var nationalExams = new List<(Exam exam, int qualificationId, int? streamId)>
            {
                // 12th Science Exams
                (new Exam
                {
                    Name = "JEE Main",
                    Description = "Joint Entrance Examination Main for Engineering",
                    DurationInMinutes = 180,
                    TotalMarks = 300,
                    PassingMarks = 100,
                    ImageUrl = "/images/exams/jee-main.jpg",
                    IsInternational = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qual12thScience?.Id ?? 0, scienceStream?.Id),

                (new Exam
                {
                    Name = "JEE Advanced",
                    Description = "Joint Entrance Examination Advanced for IITs",
                    DurationInMinutes = 180,
                    TotalMarks = 360,
                    PassingMarks = 120,
                    ImageUrl = "/images/exams/jee-advanced.jpg",
                    IsInternational = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qual12thScience?.Id ?? 0, scienceStream?.Id),

                (new Exam
                {
                    Name = "NEET",
                    Description = "National Eligibility cum Entrance Test for Medical",
                    DurationInMinutes = 180,
                    TotalMarks = 720,
                    PassingMarks = 240,
                    ImageUrl = "/images/exams/neet.jpg",
                    IsInternational = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qual12thScience?.Id ?? 0, scienceStream?.Id),

                // Graduation General Exams
                (new Exam
                {
                    Name = "UPSC Civil Services",
                    Description = "Union Public Service Commission Civil Services Examination",
                    DurationInMinutes = 180,
                    TotalMarks = 1000,
                    PassingMarks = 350,
                    ImageUrl = "/images/exams/upsc.jpg",
                    IsInternational = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qualGradGeneral?.Id ?? 0, generalStream?.Id),

                (new Exam
                {
                    Name = "SSC CGL",
                    Description = "Staff Selection Commission Combined Graduate Level",
                    DurationInMinutes = 120,
                    TotalMarks = 200,
                    PassingMarks = 70,
                    ImageUrl = "/images/exams/ssc-cgl.jpg",
                    IsInternational = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qualGradGeneral?.Id ?? 0, generalStream?.Id),

                (new Exam
                {
                    Name = "Banking PO",
                    Description = "Banking Probationary Officer Examination",
                    DurationInMinutes = 120,
                    TotalMarks = 200,
                    PassingMarks = 70,
                    ImageUrl = "/images/exams/banking-po.jpg",
                    IsInternational = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qualGradGeneral?.Id ?? 0, generalStream?.Id),

                (new Exam
                {
                    Name = "Railway NTPC",
                    Description = "Railway Non-Technical Popular Categories",
                    DurationInMinutes = 90,
                    TotalMarks = 100,
                    PassingMarks = 40,
                    ImageUrl = "/images/exams/railway-ntpc.jpg",
                    IsInternational = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qualGradGeneral?.Id ?? 0, generalStream?.Id),

                (new Exam
                {
                    Name = "Teaching CTET",
                    Description = "Central Teacher Eligibility Test",
                    DurationInMinutes = 150,
                    TotalMarks = 150,
                    PassingMarks = 60,
                    ImageUrl = "/images/exams/ctet.jpg",
                    IsInternational = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qualGradGeneral?.Id ?? 0, generalStream?.Id)
            };

            // Seed International Exams
            var internationalExams = new List<(Exam exam, int qualificationId, int? streamId)>
            {
                // 12th Science International Exams
                (new Exam
                {
                    Name = "SAT",
                    Description = "Scholastic Assessment Test for US Universities",
                    DurationInMinutes = 180,
                    TotalMarks = 1600,
                    PassingMarks = 1000,
                    ImageUrl = "/images/exams/sat.jpg",
                    IsInternational = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qual12thScience?.Id ?? 0, scienceStream?.Id),

                (new Exam
                {
                    Name = "IELTS Academic",
                    Description = "International English Language Testing System - Academic",
                    DurationInMinutes = 165,
                    TotalMarks = 9,
                    PassingMarks = 6,
                    ImageUrl = "/images/exams/ielts-academic.jpg",
                    IsInternational = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qual12thScience?.Id ?? 0, scienceStream?.Id),

                (new Exam
                {
                    Name = "TOEFL",
                    Description = "Test of English as a Foreign Language",
                    DurationInMinutes = 180,
                    TotalMarks = 120,
                    PassingMarks = 80,
                    ImageUrl = "/images/exams/toefl.jpg",
                    IsInternational = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qual12thScience?.Id ?? 0, scienceStream?.Id),

                // Graduation General International Exams
                (new Exam
                {
                    Name = "GRE",
                    Description = "Graduate Record Examination for US Graduate Schools",
                    DurationInMinutes = 210,
                    TotalMarks = 340,
                    PassingMarks = 260,
                    ImageUrl = "/images/exams/gre.jpg",
                    IsInternational = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qualGradGeneral?.Id ?? 0, generalStream?.Id),

                (new Exam
                {
                    Name = "GMAT",
                    Description = "Graduate Management Admission Test for Business Schools",
                    DurationInMinutes = 187,
                    TotalMarks = 800,
                    PassingMarks = 550,
                    ImageUrl = "/images/exams/gmat.jpg",
                    IsInternational = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qualGradGeneral?.Id ?? 0, generalStream?.Id),

                (new Exam
                {
                    Name = "PLAB",
                    Description = "Professional and Linguistic Assessments Board for UK Medical Practice",
                    DurationInMinutes = 180,
                    TotalMarks = 180,
                    PassingMarks = 120,
                    ImageUrl = "/images/exams/plab.jpg",
                    IsInternational = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qualGradGeneral?.Id ?? 0, generalStream?.Id),

                (new Exam
                {
                    Name = "OET",
                    Description = "Occupational English Test for Healthcare Professionals",
                    DurationInMinutes = 165,
                    TotalMarks = 500,
                    PassingMarks = 350,
                    ImageUrl = "/images/exams/oet.jpg",
                    IsInternational = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, qualGradGeneral?.Id ?? 0, generalStream?.Id)
            };

            // Add all exams
            var allExams = nationalExams.Concat(internationalExams).ToList();
            
            foreach (var (exam, qualificationId, streamId) in allExams)
            {
                if (qualificationId > 0 && !await _context.Exams.AnyAsync(e => e.Name == exam.Name && e.IsInternational == exam.IsInternational))
                {
                    await _context.Exams.AddAsync(exam);
                    await _context.SaveChangesAsync();

                    // Add ExamQualification relationship
                    var examQualification = new ExamQualification
                    {
                        ExamId = exam.Id,
                        QualificationId = qualificationId,
                        StreamId = streamId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.ExamQualifications.AddAsync(examQualification);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<List<QualificationDto>?> GetQualificationsFromServiceAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var qualificationServiceUrl = _configuration["Services:QualificationService:BaseUrl"] ?? "http://localhost:5011";
                
                var response = await httpClient.GetAsync($"{qualificationServiceUrl}/api/qualifications");
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<QualificationDto>>(jsonContent, options);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching qualifications: {ex.Message}");
            }
            return null;
        }

        private async Task<List<StreamDto>?> GetStreamsFromServiceAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var qualificationServiceUrl = _configuration["Services:QualificationService:BaseUrl"] ?? "http://localhost:5011";
                
                var response = await httpClient.GetAsync($"{qualificationServiceUrl}/api/streams");
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<StreamDto>>(jsonContent, options);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching streams: {ex.Message}");
            }
            return null;
        }

        private class QualificationDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public int? StreamId { get; set; }
        }

        private class StreamDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
