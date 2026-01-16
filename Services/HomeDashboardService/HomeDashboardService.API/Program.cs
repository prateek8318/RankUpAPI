using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HomeDashboardService.Application.Interfaces;
using HomeDashboardService.Application.Mappings;
using HomeDashboardService.Application.Services;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;
using HomeDashboardService.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddDbContext<HomeDashboardDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("HomeDashboardServiceConnection");
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        sqlServerOptions.CommandTimeout(60);
        sqlServerOptions.MigrationsAssembly(typeof(HomeDashboardDbContext).Assembly.GetName().Name);
    });
});

// Register repositories
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IChapterRepository, ChapterRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionOptionRepository, QuestionOptionRepository>();
builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
builder.Services.AddScoped<ILeaderboardEntryRepository, LeaderboardEntryRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IHomeBannerRepository, HomeBannerRepository>();
builder.Services.AddScoped<IPracticeModeRepository, PracticeModeRepository>();
builder.Services.AddScoped<IDailyTargetRepository, DailyTargetRepository>();
builder.Services.AddScoped<IRapidFireTestRepository, RapidFireTestRepository>();
builder.Services.AddScoped<IFreeTestRepository, FreeTestRepository>();
builder.Services.AddScoped<IMotivationMessageRepository, MotivationMessageRepository>();
builder.Services.AddScoped<ISubscriptionBannerRepository, SubscriptionBannerRepository>();
builder.Services.AddScoped<IContinuePracticeItemRepository, ContinuePracticeItemRepository>();
builder.Services.AddScoped<IOfferBannerRepository, OfferBannerRepository>();
builder.Services.AddScoped<IDailyVideoRepository, DailyVideoRepository>();
builder.Services.AddScoped<IBulkUploadLogRepository, BulkUploadLogRepository>();

// Register application services
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<HomeDashboardDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Initializing HomeDashboardService database...");
    if (await context.Database.CanConnectAsync())
    {
        await context.Database.MigrateAsync();
        
        // Seed sample data if tables are empty
        if (!await context.PracticeModes.AnyAsync())
        {
            logger.LogInformation("Seeding sample data for HomeDashboardService...");
            
            // Seed Practice Modes
            var practiceModes = new[]
            {
                new HomeDashboardService.Domain.Entities.PracticeMode
                {
                    Name = "Mathematics Practice",
                    Description = "Practice mathematics problems",
                    IconUrl = "calculator",
                    IsActive = true,
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new HomeDashboardService.Domain.Entities.PracticeMode
                {
                    Name = "Science Practice",
                    Description = "Practice science questions",
                    IconUrl = "flask",
                    IsActive = true,
                    DisplayOrder = 2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
            await context.PracticeModes.AddRangeAsync(practiceModes);
            
            // Seed Rapid Fire Tests
            var rapidFireTests = new[]
            {
                new HomeDashboardService.Domain.Entities.RapidFireTest
                {
                    Title = "Quick Math Quiz",
                    Description = "5-minute rapid fire math test",
                    DurationSeconds = 300,
                    TotalQuestions = 10,
                    IsActive = true,
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
            await context.RapidFireTests.AddRangeAsync(rapidFireTests);
            
            // Seed Free Tests
            var freeTests = new[]
            {
                new HomeDashboardService.Domain.Entities.FreeTest
                {
                    Title = "Free Assessment Test",
                    Description = "Free diagnostic test",
                    DurationMinutes = 30,
                    TotalQuestions = 25,
                    IsActive = true,
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
            await context.FreeTests.AddRangeAsync(freeTests);
            
            await context.SaveChangesAsync();
            logger.LogInformation("Sample data seeded successfully.");
        }
        
        logger.LogInformation("Database initialization completed.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error initializing database");
}

app.Run();
