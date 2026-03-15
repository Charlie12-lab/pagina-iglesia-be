using System.ComponentModel.DataAnnotations;

namespace IglesiaNet.API.DTOs;

public record BlogPostDto(
    string Id,
    string Title,
    string Content,
    string? Excerpt,
    string Author,
    int ChurchId,
    string ChurchName,
    string? CoverImageUrl,
    List<string> ImageUrls,
    List<string> Tags,
    bool IsPublished,
    DateTime? PublishedAt,
    DateTime CreatedAt
);

public record BlogPostSummaryDto(
    string Id,
    string Title,
    string? Excerpt,
    string Author,
    int ChurchId,
    string ChurchName,
    string? CoverImageUrl,
    List<string> Tags,
    DateTime? PublishedAt
);

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
    string? Title,
    string? Content,
    string? Excerpt,
    string? Author,
    string? CoverImageUrl,
    List<string>? ImageUrls,
    List<string>? Tags,
    bool? IsPublished
);
