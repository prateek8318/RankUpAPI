using AdminService.Application.Interfaces;
using AdminService.Application.Services;
using AdminService.Application.Clients;
using AdminService.Infrastructure.Data;
using AdminService.Infrastructure.Repositories;
using AdminService.Domain.Interfaces;
using AdminService.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.Middleware;
using Common.Services;
using Common.HttpClient;

var builder = WebApplication.CreateBuilder(args);

// Add HttpContextAccessor for accessing current HTTP context in delegating handlers
builder.Services.AddHttpContextAccessor();

// Register the delegating handlers
builder.Services.AddTransient<AuthTokenDelegatingHandler>();
builder.Services.AddTransient<LanguageHeaderHandler>();

// Register common language service
builder.Services.AddScoped<ILanguageService, LanguageService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddDbContext<AdminDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AdminServiceConnection");
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        sqlServerOptions.CommandTimeout(60);
        sqlServerOptions.MigrationsAssembly(typeof(AdminDbContext).Assembly.GetName().Name);
    });
});

// Repositories
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IExportLogRepository, ExportLogRepository>();
builder.Services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();

// Application Services
builder.Services.AddScoped<IAdminService, AdminService.Application.Services.AdminService>();
builder.Services.AddScoped<IDashboardAggregationService, DashboardAggregationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();

// HTTP Client for UserService
builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:UserService:BaseUrl"] ?? "http://localhost:5002");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthTokenDelegatingHandler>()
.AddHttpMessageHandler<LanguageHeaderHandler>();

// HTTP Client for ExamService  
builder.Services.AddHttpClient<IExamServiceClient, ExamServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:ExamService:BaseUrl"] ?? "https://localhost:5003");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthTokenDelegatingHandler>()
.AddHttpMessageHandler<LanguageHeaderHandler>();

// HTTP Client for SubscriptionService
builder.Services.AddHttpClient<ISubscriptionServiceClient, SubscriptionServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:SubscriptionService:BaseUrl"] ?? "https://localhost:56925");
    client.Timeout = TimeSpan.FromSeconds(60);
})
.AddHttpMessageHandler<AuthTokenDelegatingHandler>()
.AddHttpMessageHandler<LanguageHeaderHandler>();

// HTTP Client for QuizService
builder.Services.AddHttpClient<IQuizServiceClient, QuizServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:QuizService:BaseUrl"] ?? "https://localhost:5005");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthTokenDelegatingHandler>()
.AddHttpMessageHandler<LanguageHeaderHandler>();

// HTTP Client for QuestionService
builder.Services.AddHttpClient<IQuestionServiceClient, QuestionServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:QuestionService:BaseUrl"] ?? "https://localhost:5006");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthTokenDelegatingHandler>()
.AddHttpMessageHandler<LanguageHeaderHandler>();

// HTTP Client for HomeDashboardService
builder.Services.AddHttpClient<IHomeDashboardServiceClient, HomeDashboardServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:HomeDashboardService:BaseUrl"] ?? "https://localhost:56927");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthTokenDelegatingHandler>()
.AddHttpMessageHandler<LanguageHeaderHandler>();

// HTTP Client for AnalyticsService
builder.Services.AddHttpClient<IAnalyticsServiceClient, AnalyticsServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:AnalyticsService:BaseUrl"] ?? "https://localhost:5007");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthTokenDelegatingHandler>()
.AddHttpMessageHandler<LanguageHeaderHandler>();

// HTTP Client for MasterService
builder.Services.AddHttpClient<IMasterServiceClient, MasterServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:MasterService:BaseUrl"] ?? "http://localhost:5009");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthTokenDelegatingHandler>()
.AddHttpMessageHandler<LanguageHeaderHandler>();

// HTTP Client for QualificationService (now served by MasterService)
builder.Services.AddHttpClient<IQualificationServiceClient, QualificationServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:QualificationService:BaseUrl"] ?? builder.Configuration["Services:MasterService:BaseUrl"] ?? "http://localhost:5009");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthTokenDelegatingHandler>()
.AddHttpMessageHandler<LanguageHeaderHandler>();

// Add AuthService
builder.Services.AddScoped<AuthService>();

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
            "http://localhost:56923",
            "https://localhost:56923",
            "http://192.168.1.1:56923"
        },
        ValidAudiences = new[] 
        { 
            jwtSettings["Audience"],
            "http://localhost:56923",
            "https://localhost:56923",
            "http://192.168.1.1:56923"
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins(
                "http://localhost:5176",
                "http://localhost:5173",
                "http://localhost:3000", 
                "http://localhost:8080",
                "http://192.168.1.9:5176",
                "http://192.168.1.9:3000",
                "http://192.168.1.9:8080",
                "http://192.168.1.21:5176",
                "http://192.168.1.21:3000",
                "http://192.168.1.21:8080",
                "http://192.168.1.21:5173"
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
        Title = "AdminService API", 
        Version = "v1",
        Description = "Comprehensive Admin Management API with System Administration\n\n**Features:**\n- Admin user management and authentication\n- Role and permission management\n- Dashboard aggregation and analytics\n- Audit logging and export functionality\n- Subscription plan management\n- JWT token-based authentication\n\n**Base URL:** `http://localhost:56923` or `https://localhost:56923`\n\n**Authentication:** All endpoints require JWT token in Authorization header: `Bearer {your-jwt-token}`\n\n**Note:** This service requires Admin role for most operations.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "API Support",
            Email = "support@rankup.com"
        }
    });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.\n\n**How to use:**\n1. First call authentication endpoints to get a JWT token\n2. Click the 'Authorize' button below\n3. Enter 'Bearer ' followed by your token (without quotes)\n4. Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`\n\n**Note:** Token expires after configured time period. Admin role required for most endpoints.",
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

// Remove HTTPS redirection for development to avoid CORS issues
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");
app.UseLanguage();
app.UseAuthentication();
app.UseAuthorization();

// Add custom service authentication middleware (temporarily disabled for testing)
// app.UseMiddleware<ServiceAuthMiddleware>();

app.MapControllers();

// Database initialization
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Initializing AdminService database...");
    if (await context.Database.CanConnectAsync())
    {
        await context.Database.MigrateAsync();
        logger.LogInformation("Database initialization completed.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error initializing database");
}

app.Run();
