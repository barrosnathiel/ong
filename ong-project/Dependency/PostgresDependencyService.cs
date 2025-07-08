using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ong_project.Domain.Repositories;
using ong_project.Infrastructure;
using ong_project.Infrastructure.Repositories;

namespace ong_project.Dependency;

public static class PostgresDependencyService
{
    public static IServiceCollection AddPostgresDependency(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OngDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        

        return services;
    }
}