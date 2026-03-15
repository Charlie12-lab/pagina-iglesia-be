using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IglesiaNet.API.Models.MongoDB;

public class BlogPost
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty; // HTML enriquecido

    [BsonElement("excerpt")]
    public string? Excerpt { get; set; } // Resumen corto

    [BsonElement("author")]
    public string Author { get; set; } = string.Empty;

    [BsonElement("churchId")]
    public int ChurchId { get; set; }

    [BsonElement("churchName")]
    public string ChurchName { get; set; } = string.Empty;

    [BsonElement("coverImageUrl")]
    public string? CoverImageUrl { get; set; }

    [BsonElement("imageUrls")]
    public List<string> ImageUrls { get; set; } = new();

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();

    [BsonElement("isPublished")]
    public bool IsPublished { get; set; } = false;

    [BsonElement("publishedAt")]
    public DateTime? PublishedAt { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
