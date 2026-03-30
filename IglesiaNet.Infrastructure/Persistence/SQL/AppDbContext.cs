using IglesiaNet.Domain.Blogs;
using IglesiaNet.Domain.Churches;
using IglesiaNet.Domain.Events;
using IglesiaNet.Domain.Users;
using IglesiaNet.Infrastructure.Persistence.SQL.Configurations;
using Microsoft.EntityFrameworkCore;

namespace IglesiaNet.Infrastructure.Persistence.SQL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Church> Churches { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ChurchConfiguration());
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new EventRegistrationConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new BlogPostConfiguration());
    }
}
