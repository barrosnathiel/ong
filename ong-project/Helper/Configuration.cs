using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ong_project.Helper;

public static class Configuration
{
    public static IConfiguration GetConfiguration()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

        if (!File.Exists(path))
            throw new FileNotFoundException("Arquivo 'appsettings.json' não encontrado na raiz da Lambda.");

        return new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile(path, optional: false, reloadOnChange: true)
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