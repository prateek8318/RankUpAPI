using ExamService.Application.Interfaces;
using ExamService.Application.Mappings;
using ExamService.Application.Services;
using ExamService.Infrastructure.Data;
using ExamService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.Middleware;
using Common.Services;
using Common.HttpClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add HttpContextAccessor for language service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILanguageService, LanguageService>();

// Register DbContext
builder.Services.AddDbContext<ExamDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("ExamServiceConnection");
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
        sqlServerOptions.CommandTimeout(60);
        sqlServerOptions.MigrationsAssembly(typeof(ExamDbContext).Assembly.GetName().Name);
    });
});

// Register Repositories
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IExamQualificationRepository, ExamQualificationRepository>();

// Register Application Services
builder.Services.AddScoped<IExamService, ExamService.Application.Services.ExamService>();
builder.Services.AddScoped<ExamService.API.Services.ExamDataSeedService>();

// Add HttpClient for inter-service communication
builder.Services.AddHttpClient();

// Add language header handler for inter-service calls
builder.Services.AddTransient<LanguageHeaderHandler>();
builder.Services.AddHttpClient("InternalServices")
    .AddHttpMessageHandler<LanguageHeaderHandler>();

// JWT Authentication
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

// Configure CORS
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

// Add IWebHostEnvironment for file operations
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "ExamService API", 
        Version = "v1",
        Description = "Comprehensive Exam Management API with Multi-language Support\n\n**Features:**\n- Exam CRUD operations (Create, Read, Update, Delete)\n- Filter exams by qualification, stream, and international status\n- Multi-language support (English, Hindi, Tamil, Gujarati)\n- Image upload for exams\n- JWT token-based authentication\n\n**Base URL:** `http://localhost:5003` or `https://localhost:5003`\n\n**Authentication:** Most endpoints require JWT token in Authorization header: `Bearer {your-jwt-token}`\n\n**Language Support:** All GET endpoints support `language` query parameter (e.g., `?language=hi`). If not provided, language is detected from `X-Language` header or defaults to English.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "API Support",
            Email = "support@rankup.com"
        }
    });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.\n\n**How to use:**\n1. First call authentication endpoints to get a JWT token\n2. Click the 'Authorize' button below\n3. Enter 'Bearer ' followed by your token (without quotes)\n4. Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`\n\n**Note:** Token expires after configured time period",
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
    
    // Include XML Comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Disable HTTPS redirection in development to avoid 307 redirects
// app.UseHttpsRedirection();

app.UseStaticFiles(); // Serve static files from wwwroot
app.UseCors();
app.UseLanguage(); // Add language middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Initialize database
try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ExamDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Initializing ExamService database...");
    
    if (await context.Database.CanConnectAsync())
    {
        logger.LogInformation("Applying migrations...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database initialization completed.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while initializing the database");
}

app.Run();
