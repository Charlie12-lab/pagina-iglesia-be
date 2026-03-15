using IglesiaNet.Domain.Shared;

namespace IglesiaNet.Domain.Blogs;

public sealed class BlogPublication : ValueObject
{
    public bool IsPublished { get; }
    public DateTime? PublishedAt { get; }

    private BlogPublication(bool isPublished, DateTime? publishedAt)
    {
        IsPublished = isPublished;
        PublishedAt = publishedAt;
    }

    public static BlogPublication Draft() => new(false, null);

    public static BlogPublication Published() => new(true, DateTime.UtcNow);

    public BlogPublication Publish() =>
        IsPublished ? this : new BlogPublication(true, DateTime.UtcNow);

    public BlogPublication Unpublish() =>
        IsPublished ? new BlogPublication(false, PublishedAt) : this;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return IsPublished;
        yield return PublishedAt;
    }
}
