using System.Text.Json;
using IglesiaNet.Domain.Blogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IglesiaNet.Infrastructure.Persistence.SQL.Configurations;

public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    private static readonly JsonSerializerOptions _json = new();

    private static readonly ValueComparer<List<string>> _listComparer = new(
        (a, b) => a != null && b != null && a.SequenceEqual(b),
        v => v.Aggregate(0, (h, s) => HashCode.Combine(h, s.GetHashCode())),
        v => v.ToList());

    public void Configure(EntityTypeBuilder<BlogPost> b)
    {
        b.ToTable("BlogPosts");

        b.HasKey(p => p.Id);
        b.Property(p => p.Id).HasMaxLength(32).ValueGeneratedNever();

        b.Property(p => p.Title).IsRequired().HasMaxLength(500);
        b.Property(p => p.Content).IsRequired().HasColumnType("text");
        b.Property(p => p.Excerpt).HasMaxLength(1000);
        b.Property(p => p.Author).IsRequired().HasMaxLength(200);
        b.Property(p => p.ChurchId);
        b.Property(p => p.ChurchName).IsRequired().HasMaxLength(300);
        b.Property(p => p.CoverImageUrl).HasMaxLength(1000);
        b.Property(p => p.Category).HasMaxLength(100);
        b.Property(p => p.CreatedAt);
        b.Property(p => p.UpdatedAt);

        // Listas serializadas como JSON en una columna text
        b.Property(p => p.ImageUrls)
            .HasConversion(
                v => JsonSerializer.Serialize(v, _json),
                v => JsonSerializer.Deserialize<List<string>>(v, _json) ?? new List<string>())
            .HasColumnType("text")
            .Metadata.SetValueComparer(_listComparer);

        b.Property(p => p.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, _json),
                v => JsonSerializer.Deserialize<List<string>>(v, _json) ?? new List<string>())
            .HasColumnType("text")
            .Metadata.SetValueComparer(_listComparer);

        // BlogPublication como owned (columnas planas: IsPublished, PublishedAt)
        b.OwnsOne(p => p.Publication, pub =>
        {
            pub.Property(p => p.IsPublished).HasColumnName("IsPublished");
            pub.Property(p => p.PublishedAt).HasColumnName("PublishedAt");
        });
    }
}
