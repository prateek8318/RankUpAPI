using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using GatewayAPI.Middleware;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Console.WriteLine("Starting Gateway API...");

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
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

    // Add Ocelot services (use Docker config when OCELOT_CONFIG env is set)
    var ocelotFile = Environment.GetEnvironmentVariable("OCELOT_CONFIG") ?? "ocelot.json";
    ocelotFile = Path.GetFileName(ocelotFile);
    Console.WriteLine($"Using Ocelot config: {ocelotFile}");
    builder.Configuration.AddJsonFile(ocelotFile, optional: false, reloadOnChange: true);
    builder.Services.AddOcelot();

    var app = builder.Build();

    Console.WriteLine("Application built successfully");

    // Use CORS before Ocelot
    app.UseCors("AllowFrontend");

    // Configure middleware pipeline
    // app.UseLanguageValidation(); // Temporarily disabled

    Console.WriteLine("Configuring Ocelot middleware...");
    // Configure Ocelot middleware
    await app.UseOcelot();

    Console.WriteLine("Gateway API started successfully");
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Error starting Gateway API: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}
