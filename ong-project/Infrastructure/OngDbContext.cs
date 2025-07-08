using Microsoft.EntityFrameworkCore;
using ong_project.Domain;

namespace ong_project.Infrastructure;

public class OngDbContext : DbContext
{
    public OngDbContext(DbContextOptions<OngDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OngDbContext).Assembly);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Name).IsRequired(false);
            entity.Property(e => e.Address).IsRequired(false);
            entity.Property(e => e.Cpf).IsRequired(false);
            entity.Property(e => e.ProfileImage).IsRequired(false);
            entity.Property(e => e.PhoneNumber).IsRequired(false);
            entity.Property(e => e.UserType).IsRequired(false);
            entity.Property(e => e.Token).IsRequired(false);
            
        });

        modelBuilder.Entity<Course>()
            .HasOne(c => c.User)
            .WithMany(u => u.Courses)
            .HasForeignKey(c => c.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}