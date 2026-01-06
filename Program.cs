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
using RankUpAPI.Data;
using RankUpAPI.Services;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

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
});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Register UserService
builder.Services.AddScoped<IUserService, RankUpAPI.Services.UserService>();

// Add DbContext with MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var serverVersion = ServerVersion.AutoDetect(connectionString);
    
    options.UseMySql(connectionString, serverVersion, 
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    )
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors();
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
await InitializeDatabaseAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
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
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        logger.LogInformation("Ensuring database exists...");
        bool created = await context.Database.EnsureCreatedAsync();
        logger.LogInformation(created ? "Database was created" : "Database already exists");
        
        bool canConnect = await context.Database.CanConnectAsync();
        logger.LogInformation(canConnect ? "Successfully connected to database" : "Failed to connect to database");
        
        if (!canConnect)
        {
            throw new Exception("Failed to connect to the database");
        }
        
        // Log all tables in the database
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SHOW TABLES;";
        using (var reader = await command.ExecuteReaderAsync())
        {
            logger.LogInformation("Database tables:");
            while (await reader.ReadAsync())
            {
                logger.LogInformation($"- {reader[0]}");
            }
        }

        // Ensure required columns exist on Users table; add missing columns if necessary
        var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Users';";
        using (var colReader = await checkCmd.ExecuteReaderAsync())
        {
            var existingCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            while (await colReader.ReadAsync())
            {
                existingCols.Add(colReader.GetString(0));
            }

            var alterCommands = new List<string>();
            if (!existingCols.Contains("IsActive"))
            {
                alterCommands.Add("ALTER TABLE `Users` ADD COLUMN `IsActive` TINYINT(1) NOT NULL DEFAULT 1;");
            }
            if (!existingCols.Contains("IsPhoneVerified"))
            {
                alterCommands.Add("ALTER TABLE `Users` ADD COLUMN `IsPhoneVerified` TINYINT(1) NOT NULL DEFAULT 0;");
            }
            if (!existingCols.Contains("LastLoginAt"))
            {
                alterCommands.Add("ALTER TABLE `Users` ADD COLUMN `LastLoginAt` DATETIME NULL;");
            }
            if (!existingCols.Contains("UpdatedAt"))
            {
                alterCommands.Add("ALTER TABLE `Users` ADD COLUMN `UpdatedAt` DATETIME NULL;");
            }

            foreach (var sql in alterCommands)
            {
                try
                {
                    using var alterCmd = connection.CreateCommand();
                    alterCmd.CommandText = sql;
                    logger.LogInformation($"Executing schema update: {sql}");
                    await alterCmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, $"Failed to execute schema update: {sql}");
                }
            }
        }

    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
        throw;
    }
}

// Apply pending migrations on startup
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Applying database migrations...");
    await dbContext.Database.MigrateAsync();
    logger.LogInformation("Database migration completed successfully.");
}
catch (Exception ex)
{
    // Get logger in the current scope
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating the database.");
}

app.Run();
