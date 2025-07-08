using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ong_project.Domain.Repositories;
using ong_project.Infrastructure.Repositories;

namespace ong_project.Dependency.LambdaDependency;

public abstract class DependencyInjection
{
    public static IServiceProvider Inject(IConfiguration config)
    {
        var services = new ServiceCollection();
        services.AddPostgresDependency(config);
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();

        return services.BuildServiceProvider();
    }
}