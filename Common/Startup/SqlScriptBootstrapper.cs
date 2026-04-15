using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

using System.Text.RegularExpressions;

namespace Common.Startup;

public static class SqlScriptBootstrapper
{
    public static async Task ExecuteScriptsAsync(string contentRootPath, string? connectionString, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            logger.LogWarning("Connection string missing. Skipping SQL script bootstrap.");
            return;
        }

        var scriptsDir = Path.GetFullPath(Path.Combine(contentRootPath, "..", "Scripts"));
        if (!Directory.Exists(scriptsDir))
        {
            logger.LogInformation("Scripts directory not found at {ScriptsDir}. Skipping bootstrap.", scriptsDir);
            return;
        }

        var sqlFiles = Directory
            .GetFiles(scriptsDir, "*.sql", SearchOption.TopDirectoryOnly)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (sqlFiles.Count == 0)
        {
            logger.LogInformation("No SQL script files found in {ScriptsDir}.", scriptsDir);
            return;
        }

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        foreach (var sqlFile in sqlFiles)
        {
            var script = await File.ReadAllTextAsync(sqlFile);
            var batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

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
                        CommandType = System.Data.CommandType.Text,
                        CommandTimeout = 180
                    };
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Skipping failing SQL batch in {ScriptFile}", Path.GetFileName(sqlFile));
                }
            }

            logger.LogInformation("Executed SQL bootstrap script: {ScriptFile}", Path.GetFileName(sqlFile));
        }
    }
}
