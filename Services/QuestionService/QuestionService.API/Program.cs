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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddDbContext<QuestionDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("QuestionServiceConnection");
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        sqlServerOptions.CommandTimeout(60);
        sqlServerOptions.MigrationsAssembly(typeof(QuestionDbContext).Assembly.GetName().Name);
    });
});

var connectionString = builder.Configuration.GetConnectionString("QuestionServiceConnection")
    ?? throw new InvalidOperationException("QuestionServiceConnection is not configured.");
builder.Services.AddSingleton(connectionString);

builder.Services.AddScoped<IQuestionRepository, QuestionDapperRepository>();
builder.Services.AddScoped<QuestionService.Domain.Interfaces.IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionFeatureRepository, QuestionRepository>();
builder.Services.AddScoped<QuestionApplicationService>();

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
        Description = "Question Management API\n\n**Features:**\n- Question CRUD operations\n- Question option management\n- Question categorization and tagging\n- JWT token-based authentication\n\n**Base URL:** `http://localhost:5006` or `https://localhost:5006`\n\n**Authentication:** Most endpoints require JWT token in Authorization header: `Bearer {your-jwt-token}`",
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

app.UseHttpsRedirection();
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
        Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "..", "..", "database", "Create_QuestionService_Enhanced_SPs.sql"))
    };

    var connectionString = app.Configuration.GetConnectionString("QuestionServiceConnection");
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

            await using var command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 180
            };

            await command.ExecuteNonQueryAsync();
        }

        logger.LogInformation("Stored procedures synced successfully from {ScriptPath}", scriptPath);
    }
}
