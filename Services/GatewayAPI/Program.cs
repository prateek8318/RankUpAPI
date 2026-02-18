using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using GatewayAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://192.168.1.21:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Ocelot services
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

var app = builder.Build();

// Use CORS before Ocelot
app.UseCors("AllowFrontend");

// Configure middleware pipeline
app.UseLanguageValidation();

// Configure Ocelot middleware
await app.UseOcelot();
