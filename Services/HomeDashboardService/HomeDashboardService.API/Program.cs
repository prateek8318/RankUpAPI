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

// Register repositories - using Dapper implementations
builder.Services.AddScoped<IExamRepository, ExamDapperRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectDapperRepository>();
builder.Services.AddScoped<IChapterRepository, ChapterDapperRepository>();
builder.Services.AddScoped<IQuizRepository, QuizDapperRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionDapperRepository>();
builder.Services.AddScoped<IQuestionOptionRepository, QuestionOptionDapperRepository>();
builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptDapperRepository>();
builder.Services.AddScoped<ILeaderboardEntryRepository, LeaderboardEntryDapperRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationDapperRepository>();
builder.Services.AddScoped<IHomeBannerRepository, HomeBannerDapperRepository>();
builder.Services.AddScoped<IPracticeModeRepository, PracticeModeDapperRepository>();
builder.Services.AddScoped<IDailyTargetRepository, DailyTargetDapperRepository>();
builder.Services.AddScoped<IRapidFireTestRepository, RapidFireTestDapperRepository>();
builder.Services.AddScoped<IFreeTestRepository, FreeTestDapperRepository>();
builder.Services.AddScoped<IMotivationMessageRepository, MotivationMessageDapperRepository>();
builder.Services.AddScoped<ISubscriptionBannerRepository, SubscriptionBannerDapperRepository>();
builder.Services.AddScoped<IContinuePracticeItemRepository, ContinuePracticeItemDapperRepository>();
builder.Services.AddScoped<IOfferBannerRepository, OfferBannerDapperRepository>();
builder.Services.AddScoped<IDailyVideoRepository, DailyVideoDapperRepository>();
builder.Services.AddScoped<IBulkUploadLogRepository, BulkUploadLogDapperRepository>();

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
        builder.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:8080")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "HomeDashboardService API", 
        Version = "v1",
        Description = "Home Dashboard Management API\n\n**Features:**\n- Dashboard content management\n- Exam, Subject, Chapter, Quiz management\n- Practice mode and rapid fire tests\n- Home banners and notifications\n- Leaderboard management\n- JWT token-based authentication\n\n**Base URL:** `http://localhost:56927` or `https://localhost:56927`\n\n**Authentication:** Most endpoints require JWT token in Authorization header: `Bearer {your-jwt-token}`",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "API Support",
            Email = "support@rankup.com"
        }
    });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.\n\n**How to use:**\n1. First call authentication endpoints to get a JWT token\n2. Click the 'Authorize' button below\n3. Enter 'Bearer ' followed by your token (without quotes)\n4. Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add IWebHostEnvironment for file operations
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Enable serving static files
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
        logger.LogInformation("Database connection verified.");
        // No automatic migrations - using stored procedures
        // No seeding - using stored procedures
        
        logger.LogInformation("Database initialization completed.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error initializing database");
}

app.Run();
