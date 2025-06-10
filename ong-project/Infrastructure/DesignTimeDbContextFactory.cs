using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ong_project.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OngDbContext>
{
    public OngDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<OngDbContext>();
        builder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

        return new OngDbContext(builder.Options);
    }
}