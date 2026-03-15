using IglesiaNet.API.Models.SQL;
using Microsoft.EntityFrameworkCore;

namespace IglesiaNet.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Church> Churches { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Church>(entity =>
        {
            entity.HasIndex(c => c.Name);
            entity.HasIndex(c => c.City);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.ChurchId);
            entity.HasOne(e => e.Church)
                  .WithMany(c => c.Events)
                  .HasForeignKey(e => e.ChurchId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EventRegistration>(entity =>
        {
            entity.HasIndex(r => r.EventId);
            entity.HasIndex(r => r.Email);
            entity.HasOne(r => r.Event)
                  .WithMany(e => e.Registrations)
                  .HasForeignKey(r => r.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasOne(u => u.Church)
                  .WithMany(c => c.Users)
                  .HasForeignKey(u => u.ChurchId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
