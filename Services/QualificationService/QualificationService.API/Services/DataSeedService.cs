using QualificationService.Domain.Entities;
using QualificationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace QualificationService.API.Services
{
    public class DataSeedService
    {
        private readonly QualificationDbContext _context;

        public DataSeedService(QualificationDbContext context)
        {
            _context = context;
        }

        public async Task SeedDataAsync(bool clearExisting = false)
        {
            if (clearExisting)
            {
                // Clear existing data
                _context.Qualifications.RemoveRange(await _context.Qualifications.ToListAsync());
                _context.Streams.RemoveRange(await _context.Streams.ToListAsync());
                await _context.SaveChangesAsync();
            }

            // Seed Streams
            if (!await _context.Streams.AnyAsync())
            {
                var streams = new List<Domain.Entities.Stream>
                {
                    new Domain.Entities.Stream
                    {
                        Name = "Science",
                        Description = "Science stream with Physics, Chemistry, Biology/Mathematics",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Domain.Entities.Stream
                    {
                        Name = "Commerce",
                        Description = "Commerce stream with Accountancy, Business Studies, Economics",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Domain.Entities.Stream
                    {
                        Name = "Arts",
                        Description = "Arts/Humanities stream with History, Geography, Political Science",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Domain.Entities.Stream
                    {
                        Name = "General",
                        Description = "General stream for exams that don't require specific stream",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await _context.Streams.AddRangeAsync(streams);
                await _context.SaveChangesAsync();
            }

            // Get Stream IDs
            var scienceStream = await _context.Streams.FirstOrDefaultAsync(s => s.Name == "Science");
            var commerceStream = await _context.Streams.FirstOrDefaultAsync(s => s.Name == "Commerce");
            var artsStream = await _context.Streams.FirstOrDefaultAsync(s => s.Name == "Arts");
            var generalStream = await _context.Streams.FirstOrDefaultAsync(s => s.Name == "General");

            // Seed Qualifications
            if (!await _context.Qualifications.AnyAsync())
            {
                var qualifications = new List<Qualification>
                {
                    // 10th Grade
                    new Qualification
                    {
                        Name = "10th Grade",
                        Description = "Secondary School Certificate (SSC)",
                        StreamId = generalStream?.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    // 12th Grade
                    new Qualification
                    {
                        Name = "12th Grade - Science",
                        Description = "Higher Secondary Certificate (HSC) - Science Stream",
                        StreamId = scienceStream?.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Qualification
                    {
                        Name = "12th Grade - Commerce",
                        Description = "Higher Secondary Certificate (HSC) - Commerce Stream",
                        StreamId = commerceStream?.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Qualification
                    {
                        Name = "12th Grade - Arts",
                        Description = "Higher Secondary Certificate (HSC) - Arts Stream",
                        StreamId = artsStream?.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    // Graduation
                    new Qualification
                    {
                        Name = "Graduation - Science",
                        Description = "Bachelor's Degree in Science (B.Sc.)",
                        StreamId = scienceStream?.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Qualification
                    {
                        Name = "Graduation - Commerce",
                        Description = "Bachelor's Degree in Commerce (B.Com.)",
                        StreamId = commerceStream?.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Qualification
                    {
                        Name = "Graduation - Arts",
                        Description = "Bachelor's Degree in Arts (B.A.)",
                        StreamId = artsStream?.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Qualification
                    {
                        Name = "Graduation - General",
                        Description = "Any Bachelor's Degree",
                        StreamId = generalStream?.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    // Post Graduation
                    new Qualification
                    {
                        Name = "Post Graduation - Science",
                        Description = "Master's Degree in Science (M.Sc.)",
                        StreamId = scienceStream?.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Qualification
                    {
                        Name = "Post Graduation - General",
                        Description = "Any Master's Degree",
                        StreamId = generalStream?.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await _context.Qualifications.AddRangeAsync(qualifications);
                await _context.SaveChangesAsync();
            }
        }
    }
}
