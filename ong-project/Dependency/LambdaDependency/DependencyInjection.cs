using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ong_project.Dependency.LambdaDependency;

public class DependencyInjection
{
    public static IServiceProvider Inject(IConfiguration config)
    {
        var services = new ServiceCollection();
        services.AddPostgresDependency(config);

        return services.BuildServiceProvider();
    }
}