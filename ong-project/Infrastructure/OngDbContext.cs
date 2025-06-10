using Microsoft.EntityFrameworkCore;
using ong_project.Domain.User;

namespace ong_project.Infrastructure;

public class OngDbContext : DbContext
{
    public OngDbContext(DbContextOptions<OngDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OngDbContext).Assembly);
    }
}