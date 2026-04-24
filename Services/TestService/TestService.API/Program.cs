using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Data;
using System.Text.RegularExpressions;
using TestService.Application.Interfaces;
using TestService.Application.Services;
using TestServiceAppService = TestService.Application.Services.TestService;
using TestService.Application.Mappings;
using TestService.Domain.Interfaces;
using TestService.Infrastructure.Data;
using TestService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "TestService API", 
        Version = "v1",
        Description = "Comprehensive Test Management API\n\n**Features:**\n- Test CRUD operations (Create, Read, Update, Delete)\n- Test series management\n- Practice mode management\n- User test attempt tracking\n- Test question management\n- JWT token-based authentication\n\n**Base URL:** `http://localhost:5001` or `https://localhost:5001`\n\n**Authentication:** Most endpoints require JWT token in Authorization header: `Bearer {your-jwt-token}`",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@rankup.com"
        }
    });

    // Include XML Comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.\n\n**How to use:**\n1. First call authentication endpoints to get a JWT token\n2. Click the 'Authorize' button below\n3. Enter 'Bearer ' followed by your token (without quotes)\n4. Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`\n\n**Note:** Token expires after configured time period",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database
builder.Services.AddDbContext<TestDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton(builder.Configuration.GetConnectionString("DefaultConnection"));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Repositories - using Dapper implementations
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericDapperRepository<>));
builder.Services.AddScoped<ITestRepository, TestDapperRepository>();
builder.Services.AddScoped<ITestSeriesRepository, TestSeriesDapperRepository>();
builder.Services.AddScoped<IPracticeModeRepository, PracticeModeDapperRepository>();
builder.Services.AddScoped<IExamRepository, ExamDapperRepository>();
builder.Services.AddScoped<IUserTestAttemptRepository, UserTestAttemptDapperRepository>();
builder.Services.AddScoped<ITestQuestionRepository, TestQuestionDapperRepository>();
builder.Services.AddScoped<IAttemptAnswerRepository, AttemptAnswerDapperRepository>();

// Services
builder.Services.AddScoped<TestServiceAppService>();
builder.Services.AddScoped<TestSeriesService>();
builder.Services.AddScoped<HomeDashboardService>();
builder.Services.AddScoped<TestExecutionService>();
builder.Services.AddHttpClient<ISubscriptionValidationClient, SubscriptionValidationClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:SubscriptionService:BaseUrl"] ?? "http://localhost:56925");
    client.Timeout = TimeSpan.FromSeconds(15);
});
builder.Services.AddHttpClient<IQuestionEvaluationClient, QuestionEvaluationClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:QuestionService:BaseUrl"] ?? "http://localhost:56916");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5176",
                "http://localhost:5173", 
                "http://localhost:3000", 
                "http://localhost:8080",
                "http://localhost:5174",
                "http://127.0.0.1:3000",
                "http://127.0.0.1:5173",
                "http://127.0.0.1:5176",
                "http://127.0.0.1:8080",
                "http://192.168.1.9:5176",
                "http://192.168.1.9:3000",
                "http://192.168.1.9:8080",
                "http://192.168.1.9:5173",
                "http://192.168.1.9:5174",
                "http://192.168.1.21:5176",
                "http://192.168.1.21:3000",
                "http://192.168.1.21:8080",
                "http://192.168.1.21:5173",
                "http://192.168.1.21:5174"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured")))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger in all environments for testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test Service API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at apps root
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure database connection - no automatic migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Initializing TestService database...");
    if (await context.Database.CanConnectAsync())
    {
        logger.LogInformation("Database connection verified.");
        await ExecuteStoredProcedureScriptsAsync(app, logger);
        
        logger.LogInformation("Database initialization completed.");
    }
}

app.Run();

static async Task ExecuteStoredProcedureScriptsAsync(WebApplication app, ILogger logger)
{
    var scriptPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "Scripts", "TestService_Flow4_StoredProcedures.sql"));
    if (!File.Exists(scriptPath))
    {
        logger.LogWarning("Stored procedure script not found at {ScriptPath}", scriptPath);
        return;
    }

    var connectionString = app.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        logger.LogWarning("DefaultConnection is missing; skipping stored procedure sync.");
        return;
    }

    var scriptText = await File.ReadAllTextAsync(scriptPath);
    var batches = Regex.Split(scriptText, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

    await using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();

    foreach (var batch in batches)
    {
        var sql = batch.Trim();
        if (string.IsNullOrWhiteSpace(sql))
        {
            continue;
        }

        await using var command = new SqlCommand(sql, connection)
        {
            CommandType = CommandType.Text,
            CommandTimeout = 180
        };

        await command.ExecuteNonQueryAsync();
    }

    logger.LogInformation("Stored procedures synced successfully from {ScriptPath}", scriptPath);
}
