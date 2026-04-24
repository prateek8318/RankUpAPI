using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestionService.Application.Interfaces;
using QuestionService.Application.Mappings;
using QuestionApplicationService = QuestionService.Application.Services.QuestionService;
using QuestionService.Infrastructure.Data;
using QuestionService.Infrastructure.Repositories;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
var questionServiceConnectionString = GetQuestionServiceConnectionString(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new QuestionService.Application.Serialization.BoolIntJsonConverter());
    });

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddDbContext<QuestionDbContext>(options =>
{
    options.UseSqlServer(questionServiceConnectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        sqlServerOptions.CommandTimeout(60);
        sqlServerOptions.MigrationsAssembly(typeof(QuestionDbContext).Assembly.GetName().Name);
    });
});

builder.Services.AddScoped<IQuestionRepository>(provider => {
    return new QuestionDapperRepository(questionServiceConnectionString);
});
builder.Services.AddScoped<QuestionService.Domain.Interfaces.IQuestionRepository>(provider => {
    return new QuestionRepository(questionServiceConnectionString);
});
builder.Services.AddScoped<IQuestionFeatureRepository>(provider => {
    return new QuestionRepository(questionServiceConnectionString);
});
builder.Services.AddScoped<QuestionApplicationService>();

// Mock Test Services
builder.Services.AddScoped<IMockTestRepository>(provider => {
    return new MockTestRepository(questionServiceConnectionString);
});
builder.Services.AddScoped<IMockTestService, QuestionService.Application.Services.MockTestService>();

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
        Title = "QuestionService API", 
        Version = "v1",
        Description = "Question Management API\n\n**Features:**\n- Question CRUD operations\n- Question option management\n- Question categorization and tagging\n- JWT token-based authentication\n\n**Base URL:** `http://localhost:56916` or `https://localhost:56917`\n\n**Authentication:** Most endpoints require JWT token in Authorization header: `Bearer {your-jwt-token}`",
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

var app = builder.Build();

// Enable Swagger in all environments for testing
app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<QuestionDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Initializing QuestionService database...");
    if (await context.Database.CanConnectAsync())
    {
        logger.LogInformation("Database connection verified.");
        await ExecuteStoredProcedureScriptsAsync(app, logger);
        logger.LogInformation("Database initialization completed.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error initializing database");
}

app.Run();

static async Task ExecuteStoredProcedureScriptsAsync(WebApplication app, ILogger logger)
{
    var scriptPaths = new[]
    {
        Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "Scripts", "QuestionService_StoredProcedures.sql")),
        Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "Scripts", "MockTests_Schema_Migration.sql")),
        Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "..", "..", "database", "Create_QuestionService_Enhanced_SPs.sql"))
    };

    var connectionString = GetQuestionServiceConnectionString(app.Configuration);
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        logger.LogWarning("Connection string is missing; skipping stored procedure script execution.");
        return;
    }

    await using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();

    foreach (var scriptPath in scriptPaths.Distinct())
    {
        if (!File.Exists(scriptPath))
        {
            logger.LogWarning("Stored procedure script not found at {ScriptPath}", scriptPath);
            continue;
        }

        var scriptText = await File.ReadAllTextAsync(scriptPath);
        var batches = Regex.Split(scriptText, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        foreach (var batch in batches)
        {
            var sql = batch.Trim();
            if (string.IsNullOrWhiteSpace(sql))
            {
                continue;
            }

            try
            {
                await using var command = new SqlCommand(sql, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 180
                };

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex) when (ex.Number == 207)
            {
                // Schema drift: some environments don't have newer columns (e.g. ExamId).
                // Skip failing batches so the service can still start.
                logger.LogWarning(ex, "Skipping stored procedure batch due to missing column while executing {ScriptPath}", scriptPath);
            }
            catch (SqlException ex) when (ex.Number == 2714 || ex.Number == 1913)
            {
                // Idempotent scripts may still contain CREATE statements; treat "already exists" as non-fatal.
                logger.LogInformation(ex, "Skipping stored procedure batch because object already exists while executing {ScriptPath}", scriptPath);
            }
        }

        logger.LogInformation("Stored procedures synced successfully from {ScriptPath}", scriptPath);
    }
}

static string GetQuestionServiceConnectionString(IConfiguration configuration)
{
    var rawConnectionString = configuration.GetConnectionString("QuestionServiceConnection");
    if (string.IsNullOrWhiteSpace(rawConnectionString))
    {
        throw new InvalidOperationException("QuestionServiceConnection is not configured.");
    }

    var builder = new SqlConnectionStringBuilder(rawConnectionString);
    if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
    {
        builder.InitialCatalog = "RankUp_QuestionDB";
        Console.WriteLine("QuestionServiceConnection did not include database; defaulting to RankUp_QuestionDB.");
    }

    return builder.ConnectionString;
}

static string GetMasterDbConnectionString(IConfiguration configuration)
{
    var rawConnectionString = configuration.GetConnectionString("MasterDBConnection");
    if (string.IsNullOrWhiteSpace(rawConnectionString))
    {
        throw new InvalidOperationException("MasterDBConnection is not configured.");
    }

    var builder = new SqlConnectionStringBuilder(rawConnectionString);
    if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
    {
        builder.InitialCatalog = "RankUp_MasterDB";
        Console.WriteLine("MasterDBConnection did not include database; defaulting to RankUp_MasterDB.");
    }

    return builder.ConnectionString;
}
