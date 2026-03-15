using IglesiaNet.Domain.Churches;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IglesiaNet.Infrastructure.Persistence.SQL.Configurations;

public class ChurchConfiguration : IEntityTypeConfiguration<Church>
{
    public void Configure(EntityTypeBuilder<Church> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Address).HasMaxLength(500);
        builder.Property(c => c.City).HasMaxLength(100);
        builder.Property(c => c.Phone).HasMaxLength(20);
        builder.Property(c => c.Email).HasMaxLength(200);
        builder.Property(c => c.LogoUrl).HasMaxLength(500);
        builder.Property(c => c.WebsiteUrl).HasMaxLength(500);

        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.City);
    }
}
