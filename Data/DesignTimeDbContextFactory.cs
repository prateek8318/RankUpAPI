using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace RankUpAPI.Data
{
    // Design-time factory to allow `dotnet ef` tools to create the DbContext
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                   ?? System.Environment.GetEnvironmentVariable("DefaultConnection")
                                   ?? "Server=localhost;Database=RankUpDB;User=root;Password=;";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            optionsBuilder.UseMySql(connectionString, serverVersion);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
