using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ong_project.Helper;

public static class Configuration
{
    public static IConfiguration GetConfiguration()
    {
        var basePath = Directory.GetCurrentDirectory();;
        var configPath = FindFileInParentDirs(basePath, Path.Combine("ong-project", "appsettings.json"));

        if (configPath == null)
            throw new FileNotFoundException("Não foi possível localizar o arquivo 'appsettings.json' na pasta 'ong-project'.");

        return new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile(configPath, optional: false, reloadOnChange: true)
            .Build();
    }

    private static string? FindFileInParentDirs(string startPath, string relativePath)
    {
        var dir = new DirectoryInfo(startPath);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, relativePath);
            if (File.Exists(candidate))
                return candidate;

            dir = dir.Parent;
        }
        return null;
    }
}