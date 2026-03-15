using System.ComponentModel.DataAnnotations;
using IglesiaNet.Domain.Blogs;

namespace IglesiaNet.Application.Blogs;

public record BlogPostDto(
    string Id, string Title, string Content, string? Excerpt,
    string Author, int ChurchId, string ChurchName,
    string? CoverImageUrl, List<string> ImageUrls, List<string> Tags,
    bool IsPublished, DateTime? PublishedAt, DateTime CreatedAt
)
{
    public static BlogPostDto From(BlogPost b) => new(
        b.Id, b.Title, b.Content, b.Excerpt, b.Author,
        b.ChurchId, b.ChurchName, b.CoverImageUrl, b.ImageUrls, b.Tags,
        b.Publication.IsPublished, b.Publication.PublishedAt, b.CreatedAt);
}

public record BlogPostSummaryDto(
    string Id, string Title, string? Excerpt, string Author,
    int ChurchId, string ChurchName, string? CoverImageUrl,
    List<string> Tags, DateTime? PublishedAt
)
{
    public static BlogPostSummaryDto From(BlogPost b) => new(
        b.Id, b.Title, b.Excerpt, b.Author,
        b.ChurchId, b.ChurchName, b.CoverImageUrl,
        b.Tags, b.Publication.PublishedAt);
}

public record CreateBlogPostRequest(
    [Required, MaxLength(300)] string Title,
    [Required] string Content,
    string? Excerpt,
    [Required] string Author,
    [Required] int ChurchId,
    string? CoverImageUrl,
    List<string>? ImageUrls,
    List<string>? Tags,
    bool IsPublished
);

public record UpdateBlogPostRequest(
    string? Title, string? Content, string? Excerpt, string? Author,
    string? CoverImageUrl, List<string>? ImageUrls, List<string>? Tags, bool? IsPublished
);
