using IglesiaNet.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IglesiaNet.Infrastructure.Persistence.SQL.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Location).HasMaxLength(300);
        builder.Property(e => e.ImageUrl).HasMaxLength(500);
        builder.Property(e => e.Price).HasColumnType("decimal(10,2)");

        // Owned Value Objects — se mapean a columnas de la misma tabla
        builder.OwnsOne(e => e.Schedule, s =>
        {
            s.Property(x => x.StartDate).HasColumnName("StartDate").IsRequired();
            s.Property(x => x.EndDate).HasColumnName("EndDate");
        });

        builder.OwnsOne(e => e.Capacity, c =>
        {
            c.Property(x => x.AllowsRegistration).HasColumnName("AllowsRegistration");
            c.Property(x => x.MaxAttendees).HasColumnName("MaxAttendees");
        });

        builder.HasIndex(e => new { e.ChurchId });

        // Colección de registraciones como owned entities
        builder.HasMany(e => e.Registrations)
               .WithOne()
               .HasForeignKey(r => r.EventId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class EventRegistrationConfiguration : IEntityTypeConfiguration<EventRegistration>
{
    public void Configure(EntityTypeBuilder<EventRegistration> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.FullName).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Email).HasMaxLength(200);  // nullable — miembros grupales
        builder.Property(r => r.Phone).HasMaxLength(50);
        builder.Property(r => r.Church).HasMaxLength(300);
        builder.Property(r => r.VoucherPath).HasMaxLength(500);
        builder.Property(r => r.GroupId).HasMaxLength(36);

        builder.HasIndex(r => r.EventId);
        builder.HasIndex(r => r.Email);
    }
}
