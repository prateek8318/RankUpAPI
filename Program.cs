using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RankUpAPI.Areas.Admin.Services.Implementations;
using RankUpAPI.Areas.Admin.Services.Interfaces;
using RankUpAPI.Core.Configuration;
using RankUpAPI.Core.Services.Interfaces;
using RankUpAPI.Core.Services.Implementations;
using RankUpAPI.Data;
using RankUpAPI.Services;
using RankUpAPI.Repositories.Interfaces;
using RankUpAPI.Repositories.Implementations;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;

// Set default culture
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Create the web application builder
var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container
builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    // Handle JSON serialization
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    // Allow enums (like HomeSectionType) to be sent/received as strings: "MockTest"
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Register AutoMapper with our MappingProfile
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<RankUpAPI.Mappings.MappingProfile>();
}, AppDomain.CurrentDomain.GetAssemblies());

// Configure OTP Settings
builder.Services.Configure<OtpSettings>(
    builder.Configuration.GetSection(OtpSettings.SectionName));

// Register Core Services
builder.Services.AddScoped<IOtpService, OtpService>();

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IQualificationRepository, QualificationRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IExamQualificationRepository, ExamQualificationRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IChapterRepository, ChapterRepository>();
builder.Services.AddScoped<ITestSeriesRepository, TestSeriesRepository>();
builder.Services.AddScoped<ITestSeriesQuestionRepository, TestSeriesQuestionRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IHomeSectionItemRepository, HomeSectionItemRepository>();

// Register Business Services
builder.Services.AddScoped<IUserService, RankUpAPI.Services.UserService>();

// Register Admin Area Services (Entity-based structure)
builder.Services.AddScoped<RankUpAPI.Areas.Admin.Exam.Services.Interfaces.IExamService, RankUpAPI.Areas.Admin.Exam.Services.Implementations.ExamService>();
builder.Services.AddScoped<RankUpAPI.Areas.Admin.Qualification.Services.Interfaces.IQualificationService, RankUpAPI.Areas.Admin.Qualification.Services.Implementations.QualificationService>();
builder.Services.AddScoped<RankUpAPI.Areas.Admin.Subject.Services.Interfaces.ISubjectService, RankUpAPI.Areas.Admin.Subject.Services.Implementations.SubjectService>();
builder.Services.AddScoped<RankUpAPI.Areas.Admin.Chapter.Services.Interfaces.IChapterService, RankUpAPI.Areas.Admin.Chapter.Services.Implementations.ChapterService>();
builder.Services.AddScoped<RankUpAPI.Areas.Admin.Question.Services.Interfaces.IQuestionService, RankUpAPI.Areas.Admin.Question.Services.Implementations.QuestionService>();
builder.Services.AddScoped<RankUpAPI.Areas.Admin.TestSeries.Services.Interfaces.ITestSeriesService, RankUpAPI.Areas.Admin.TestSeries.Services.Implementations.TestSeriesService>();
builder.Services.AddScoped<RankUpAPI.Areas.Admin.HomeContent.Services.Interfaces.IHomeContentService, RankUpAPI.Areas.Admin.HomeContent.Services.Implementations.HomeContentService>();

// Register User Area Services
builder.Services.AddScoped<RankUpAPI.Areas.Users.HomeContent.Services.Interfaces.IHomeContentService, RankUpAPI.Areas.Users.HomeContent.Services.Implementations.HomeContentService>();

// Add DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    logger.LogInformation($"Using connection string: {connectionString}");
    
    try
    {
        options.UseSqlServer(connectionString, 
            sqlServerOptions => 
            {
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                sqlServerOptions.CommandTimeout(60);
                // Use the assembly that contains the DbContext for migrations to avoid hard-coded project names
                sqlServerOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name);
            })
        .LogTo(message => 
        {
            try 
            {
                System.IO.File.AppendAllText("database_log.txt", $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] {message}{Environment.NewLine}");
                Console.WriteLine($"[DB] {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }, 
        new[] { DbLoggerCategory.Database.Command.Name }, 
        LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to configure database context");
        throw;
    }
});

// Add support for API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    var defaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.DefaultPolicy = defaultPolicy;

    options.AddPolicy("RequireAdminRole", policy => 
        policy.RequireRole("Admin"));
});

// Add Admin Auth Service
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

// Add Admin credentials to configuration
builder.Configuration.GetSection("AdminCredentials").Bind(new
{
    Email = "admin@rankup.com",
    Password = "Admin@123"
});

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

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Build the application
var app = builder.Build();

// Initialize the database
try
{
    await InitializeDatabaseAsync(app);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while initializing the database");
    throw;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Enable static files (for wwwroot/images, etc.)
app.UseStaticFiles();

app.UseRouting();

// Enable CORS before other middleware
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run the application
app.Run();

async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try 
    {
        logger.LogInformation("Initializing database...");
        
        using var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Check if database exists
        var databaseExists = await context.Database.CanConnectAsync();
        logger.LogInformation($"Database exists: {databaseExists}");
        
        // Apply migrations
        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();
        
        logger.LogInformation("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
        throw; // Re-throw to stop the application if database initialization fails
    }
}
