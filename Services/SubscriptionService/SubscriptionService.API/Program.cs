using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Application.Mappings;
using SubscriptionService.Application.Services;
using SubscriptionService.Domain.Interfaces;
using SubscriptionService.Infrastructure.Data;
using SubscriptionService.Infrastructure.Repositories;
using SubscriptionService.Infrastructure.Services;
using Swashbuckle.AspNetCore.Swagger;
using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using Common.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddDbContext<SubscriptionDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SubscriptionServiceConnection");
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        sqlServerOptions.CommandTimeout(60);
        sqlServerOptions.MigrationsAssembly(typeof(SubscriptionDbContext).Assembly.GetName().Name);
    });
});

// Register connection string for Dapper repositories
var connectionString = builder.Configuration.GetConnectionString("SubscriptionServiceConnection");
builder.Services.AddSingleton<string>(connectionString);

// Register repositories
builder.Services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanDapperRepository>();
builder.Services.AddScoped<IUserSubscriptionRepository, UserSubscriptionDapperRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IDemoAccessLogRepository, DemoAccessLogRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();

// Add HttpContextAccessor for language service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Common.Services.ILanguageService, Common.Services.LanguageService>();

// Register domain services
builder.Services.AddScoped<IRazorpayService, RazorpayService>();
builder.Services.AddScoped<ISubscriptionValidationService, SubscriptionService.Domain.Services.SubscriptionValidationService>();

// Register application services
builder.Services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();
builder.Services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ISubscriptionValidationAppService, SubscriptionValidationAppService>();
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();

// Register HttpClient for Razorpay
builder.Services.AddHttpClient<IRazorpayService, RazorpayService>();

// Register HttpClientFactory for inter-service communication
builder.Services.AddHttpClient();

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
        builder.WithOrigins(
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "SubscriptionService API", 
        Version = "v1",
        Description = "Comprehensive Subscription Plan Management API with Multi-language Support\n\n**Features:**\n- Subscription plan CRUD operations (Create, Read, Update, Delete)\n- Multi-language support (English, Hindi, Tamil, Gujarati) - defaults to English if language not provided\n- Plan features include: Name, Description, Price, Currency, Duration, Features, Card Color Theme, Active/Recommended/Popular flags\n- Filter plans by exam category\n- JWT token-based authentication\n\n**Base URL:** `http://localhost:5004` or `https://localhost:5004`\n\n**Authentication:** Most endpoints require JWT token in Authorization header: `Bearer {your-jwt-token}`\n\n**Language Support:** All GET endpoints support `language` query parameter (e.g., `?language=hi`). If not provided, language is detected from `X-Language` header or defaults to English. All text fields (Name, Description, Features) are localized.",
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
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

var app = builder.Build();

// Enable Swagger in all environments for testing
app.UseSwagger();
app.UseSwaggerUI();

// Serve wwwroot (uploads etc.)
app.UseStaticFiles();

// Remove HTTPS redirection for development to avoid CORS issues
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors();
app.UseLanguage(); // Add language middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SubscriptionDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var swaggerProvider = scope.ServiceProvider.GetRequiredService<ISwaggerProvider>();
    
    logger.LogInformation("Initializing SubscriptionService database...");
    if (await context.Database.CanConnectAsync())
    {
        await context.Database.MigrateAsync();
        await ExecuteStoredProcedureScriptAsync(app, logger);
        await GeneratePostmanCollectionAsync(app, logger, swaggerProvider);
        logger.LogInformation("Database initialization completed.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error initializing database");
}

app.Run();

static async Task ExecuteStoredProcedureScriptAsync(WebApplication app, ILogger logger)
{
    var contentRoot = app.Environment.ContentRootPath;
    var scriptPath = Path.GetFullPath(Path.Combine(contentRoot, "..", "Scripts", "SubscriptionService_StoredProcedures.sql"));
    if (!File.Exists(scriptPath))
    {
        logger.LogWarning("Stored procedure script not found at {ScriptPath}", scriptPath);
        return;
    }

    var connectionString = app.Configuration.GetConnectionString("SubscriptionServiceConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        logger.LogWarning("Connection string is missing; skipping stored procedure script execution.");
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
            CommandType = System.Data.CommandType.Text,
            CommandTimeout = 180
        };

        await command.ExecuteNonQueryAsync();
    }

    logger.LogInformation("Stored procedures synced successfully from {ScriptPath}", scriptPath);
}

static async Task GeneratePostmanCollectionAsync(WebApplication app, ILogger logger, ISwaggerProvider swaggerProvider)
{
    var openApi = swaggerProvider.GetSwagger("v1");
    var baseUrlValue = app.Configuration["Postman:BaseUrl"] ?? "http://localhost:5004";

    var items = new List<object>();
    foreach (var (path, pathItem) in openApi.Paths)
    {
        var pathRequests = new List<object>();
        foreach (var (method, operation) in pathItem.Operations)
        {
            var route = path.StartsWith("/") ? path : "/" + path;
            pathRequests.Add(new
            {
                name = operation.Summary ?? $"{method.ToString().ToUpperInvariant()} {route}",
                request = new
                {
                    method = method.ToString().ToUpperInvariant(),
                    header = new object[]
                    {
                        new { key = "Content-Type", value = "application/json", type = "text" }
                    },
                    url = new
                    {
                        raw = "{{baseUrl}}" + route
                    },
                    description = operation.Description ?? operation.Summary ?? string.Empty
                },
                response = Array.Empty<object>()
            });
        }

        items.Add(new
        {
            name = path,
            item = pathRequests
        });
    }

    var collection = new
    {
        info = new
        {
            name = "RankUpAPI SubscriptionService",
            _postman_id = Guid.NewGuid().ToString(),
            schema = "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
        },
        item = items,
        variable = new object[]
        {
            new { key = "baseUrl", value = baseUrlValue }
        }
    };

    var repoRoot = Directory.GetParent(app.Environment.ContentRootPath)?.Parent?.Parent?.FullName;
    var outputPath = string.IsNullOrWhiteSpace(repoRoot)
        ? Path.Combine(app.Environment.ContentRootPath, "SubscriptionService-Postman-Collection.json")
        : Path.Combine(repoRoot, "RankUpAPI-Postman-Collection.json");

    var json = JsonSerializer.Serialize(collection, new JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(outputPath, json);
    logger.LogInformation("Postman collection refreshed at {OutputPath}", outputPath);
}
