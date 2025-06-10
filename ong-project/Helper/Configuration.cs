using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ong_project.Helper;

public static class Configuration
{
    public static IConfiguration GetConfiguration()
    {
        var basePath = Directory.GetCurrentDirectory();
        var possiblePaths = new[]
        {
            Path.Combine(basePath, "appsettings.json"), // For Lambda
            Path.Combine(basePath, "..", "ong-project", "appsettings.json"), // For local dev
            Path.Combine(basePath, "..", "..", "ong-project", "appsettings.json") // Alternative local path
        };

        var configBuilder = new ConfigurationBuilder()
            .AddEnvironmentVariables(); // Always include environment variables

        // Add the first found JSON file
        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                configBuilder.AddJsonFile(path, optional: false);
                break;
            }
        }

        return configBuilder.Build();
    }
}