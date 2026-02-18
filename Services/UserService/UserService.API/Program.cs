using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Application.Mappings;
using UserService.Application.Services;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories;
using Common.Middleware;
using Common.Services;
using Common.HttpClient;
using Common.Language;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add HttpContextAccessor for language service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILanguageService, LanguageService>();

builder.Services.AddDbContext<UserDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("UserServiceConnection");
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        sqlServerOptions.CommandTimeout(60);
        sqlServerOptions.MigrationsAssembly("UserService.API");
    });
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService.Application.Services.UserService>();
builder.Services.AddSingleton<IOtpService, UserService.Application.Services.OtpService>();
builder.Services.AddScoped<IImageService, UserService.Infrastructure.Services.ImageService>();
builder.Services.AddScoped<IUserLanguageService, UserService.Application.Services.UserLanguageService>();
builder.Services.AddScoped<ILanguageDataService, Common.Language.LanguageDataService>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = builder.Environment.IsDevelopment() ? false : true;
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
        ClockSkew = TimeSpan.Zero,
        // Allow development URLs for testing
        ValidIssuers = new[] 
        { 
            jwtSettings["Issuer"],
            "http://localhost:5002",
            "https://localhost:5002",
            "http://192.168.1.1:5002"
        },
        ValidAudiences = new[] 
        { 
            jwtSettings["Audience"],
            "http://localhost:5002",
            "https://localhost:5002",
            "http://192.168.1.1:5002"
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://127.0.0.1:3000")
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
        Title = "UserService API", 
        Version = "v1",
        Description = "Comprehensive User Management API with OTP-based Authentication, Profile Management, and Multi-language Support\n\n**Features:**\n- OTP-based mobile authentication\n- User profile management with photo upload\n- Multi-language data support (states, qualifications, categories, streams)\n- JWT token-based authentication\n- International exam preferences\n\n**Base URL:** `http://localhost:5002` or `https://localhost:5002`\n\n**Authentication:** Most endpoints require JWT token in Authorization header: `Bearer {your-jwt-token}`",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "API Support",
            Email = "support@rankup.com"
        }
    });
    
    // Include XML Comments for better documentation
    var xmlFile = "UserService.API.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.\n\n**How to use:**\n1. First call `/api/users/auth/send-otp` and `/api/users/auth/verify-otp` to get a JWT token\n2. Click the 'Authorize' button below\n3. Enter 'Bearer ' followed by your token (without quotes)\n4. Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`\n\n**Note:** Token expires after 60 minutes",
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Disabled for development to avoid 307 redirects
app.UseStaticFiles(); // Enable serving static files
app.UseCors();
app.UseLanguage(); // Add language middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Initializing UserService database...");
    if (await context.Database.CanConnectAsync())
    {
        await context.Database.MigrateAsync();
        
        // Seed sample data if no users exist
        if (!await context.Users.AnyAsync())
        {
            logger.LogInformation("Seeding sample users...");
            var otpService = scope.ServiceProvider.GetRequiredService<IOtpService>();
            
            var sampleUsers = new[]
            {
                new UserService.Domain.Entities.User
                {
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "1234567890",
                    IsActive = true,
                    IsPhoneVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow.AddHours(-2)
                },
                new UserService.Domain.Entities.User
                {
                    Name = "Jane Smith",
                    Email = "jane.smith@example.com",
                    PhoneNumber = "9876543210",
                    IsActive = true,
                    IsPhoneVerified = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1),
                    LastLoginAt = DateTime.UtcNow.AddDays(-1)
                },
                new UserService.Domain.Entities.User
                {
                    Name = "Bob Johnson",
                    Email = "bob.johnson@example.com",
                    PhoneNumber = "5555555555",
                    IsActive = false,
                    IsPhoneVerified = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    LastLoginAt = DateTime.UtcNow.AddDays(-3)
                }
            };
            
            await context.Users.AddRangeAsync(sampleUsers);
            await context.SaveChangesAsync();
            logger.LogInformation("Sample users seeded successfully.");
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
