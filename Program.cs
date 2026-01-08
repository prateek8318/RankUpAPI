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
using System.Globalization;
using System.Reflection;

// Ensure globalization invariant is disabled (if it was set in environment), to avoid CultureNotFoundException
try
{
    Environment.SetEnvironmentVariable("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "false");
}
catch { }

// Ensure invariant culture for threads (fallback)
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

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
                // Use the executing assembly name for migrations to match runtime assembly
                sqlServerOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
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
        
        // Try to connect
        var canConnect = await context.Database.CanConnectAsync();
        logger.LogInformation($"Database can connect: {canConnect}");

        // Attempt to open a direct connection and log details
        try
        {
            var connection = context.Database.GetDbConnection();
            await connection.OpenAsync();

            logger.LogInformation($"Connected to DB. DataSource: {connection.DataSource}, Database: {connection.Database}");
            try
            {
                logger.LogInformation($"Server Version: {connection.ServerVersion}");
            }
            catch { /* ignore if provider doesn't support ServerVersion */ }

            // List user tables
            try
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME;";
                using var reader = await cmd.ExecuteReaderAsync();
                logger.LogInformation("Database tables:");
                System.IO.File.AppendAllText("database_log.txt", $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] Connected to DB: {connection.DataSource} | Database: {connection.Database}{Environment.NewLine}");
                while (await reader.ReadAsync())
                {
                    var schema = reader.GetString(0);
                    var name = reader.GetString(1);
                    logger.LogInformation($"- {schema}.{name}");
                    System.IO.File.AppendAllText("database_log.txt", $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] Table: {schema}.{name}{Environment.NewLine}");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to list tables from INFORMATION_SCHEMA.TABLES");
                System.IO.File.AppendAllText("database_log.txt", $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] Failed to list tables: {ex.Message}{Environment.NewLine}");
            }

            // Ensure Users table has required columns (add missing columns for SQL Server)
            try
            {
                using var colCmd = connection.CreateCommand();
                colCmd.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND TABLE_CATALOG = DB_NAME();";
                var existingCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var colReader = await colCmd.ExecuteReaderAsync())
                {
                    while (await colReader.ReadAsync())
                    {
                        existingCols.Add(colReader.GetString(0));
                    }
                }

                var alterCommands = new List<string>();
                if (!existingCols.Contains("DateOfBirth"))
                {
                    alterCommands.Add("ALTER TABLE [Users] ADD [DateOfBirth] DATE NULL;");
                }
                if (!existingCols.Contains("LanguagePreference"))
                {
                    alterCommands.Add("ALTER TABLE [Users] ADD [LanguagePreference] VARCHAR(50) NULL;");
                }
                if (!existingCols.Contains("Qualification"))
                {
                    alterCommands.Add("ALTER TABLE [Users] ADD [Qualification] VARCHAR(100) NULL;");
                }

                foreach (var sql in alterCommands)
                {
                    try
                    {
                        using var alterCmd = connection.CreateCommand();
                        alterCmd.CommandText = sql;
                        logger.LogInformation($"Executing schema update: {sql}");
                        await alterCmd.ExecuteNonQueryAsync();
                        System.IO.File.AppendAllText("database_log.txt", $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] Executed schema update: {sql}{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, $"Failed to execute schema update: {sql}");
                        System.IO.File.AppendAllText("database_log.txt", $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] Failed schema update: {sql} - {ex.Message}{Environment.NewLine}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to verify or modify Users table columns");
                System.IO.File.AppendAllText("database_log.txt", $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] Failed to verify/modify Users columns: {ex.Message}{Environment.NewLine}");
            }

            await connection.CloseAsync();
        }
        catch (Exception connEx)
        {
            logger.LogError(connEx, "Failed to open DB connection");
            System.IO.File.AppendAllText("database_log.txt", $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] DB connection error: {connEx.Message}{Environment.NewLine}");
        }
        
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
