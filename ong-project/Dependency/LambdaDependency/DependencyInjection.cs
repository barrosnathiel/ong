using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ong_project.Dependency.LambdaDependency;

public class DependencyInjection
{
    public static IServiceProvider Inject()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var services = new ServiceCollection();
        services.AddPostgresDependency(config);

        return services.BuildServiceProvider();
    }
}