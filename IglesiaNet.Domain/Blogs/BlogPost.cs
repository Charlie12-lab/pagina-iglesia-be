using IglesiaNet.Domain.Shared;

namespace IglesiaNet.Domain.Blogs;

public class BlogPost : Entity<string>
{
    public string Title { get; private set; }
    public string Content { get; private set; }
    public string? Excerpt { get; private set; }
    public string Author { get; private set; }
    public int ChurchId { get; private set; }
    public string ChurchName { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public List<string> ImageUrls { get; private set; }
    public List<string> Tags { get; private set; }
    public BlogPublication Publication { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Para MongoDB / EF sin constructores públicos
    private BlogPost() : base()
    {
        Title = string.Empty; Content = string.Empty;
        Author = string.Empty; ChurchName = string.Empty;
        ImageUrls = new(); Tags = new(); Publication = BlogPublication.Draft();
    }

    private BlogPost(string id, string title, string content, string? excerpt,
        string author, int churchId, string churchName,
        string? coverImageUrl, List<string> imageUrls, List<string> tags,
        bool isPublished) : base(id)
    {
        Title = title; Content = content; Excerpt = excerpt;
        Author = author; ChurchId = churchId; ChurchName = churchName;
        CoverImageUrl = coverImageUrl; ImageUrls = imageUrls; Tags = tags;
        Publication = isPublished ? BlogPublication.Published() : BlogPublication.Draft();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static BlogPost Create(
        string title, string content, string? excerpt,
        string author, int churchId, string churchName,
        string? coverImageUrl, List<string>? imageUrls, List<string>? tags, bool isPublished)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("El título del blog es requerido");
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("El contenido no puede estar vacío");
        if (string.IsNullOrWhiteSpace(author))
            throw new DomainException("El autor es requerido");

        var id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        return new BlogPost(id, title.Trim(), content, excerpt?.Trim(),
            author.Trim(), churchId, churchName,
            coverImageUrl?.Trim(), imageUrls ?? new(), tags ?? new(), isPublished);
    }

    public void Update(string? title, string? content, string? excerpt,
        string? author, string? coverImageUrl, List<string>? imageUrls, List<string>? tags, bool? isPublished)
    {
        if (title is not null)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new DomainException("El título no puede estar vacío");
            Title = title.Trim();
        }
        if (content is not null)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new DomainException("El contenido no puede estar vacío");
            Content = content;
        }
        if (excerpt is not null) Excerpt = excerpt.Trim();
        if (author is not null) Author = author.Trim();
        if (coverImageUrl is not null) CoverImageUrl = coverImageUrl.Trim();
        if (imageUrls is not null) ImageUrls = imageUrls;
        if (tags is not null) Tags = tags;
        if (isPublished.HasValue) Publication = isPublished.Value
            ? Publication.Publish()
            : Publication.Unpublish();

        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish() { Publication = Publication.Publish(); UpdatedAt = DateTime.UtcNow; }
    public void Unpublish() { Publication = Publication.Unpublish(); UpdatedAt = DateTime.UtcNow; }
}
