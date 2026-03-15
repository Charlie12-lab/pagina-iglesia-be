using IglesiaNet.Domain.Blogs;
using global::MongoDB.Driver;

namespace IglesiaNet.Infrastructure.Persistence.MongoDB;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly MongoDbContext _mongo;
    public BlogPostRepository(MongoDbContext mongo) => _mongo = mongo;

    public async Task<BlogPost?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await _mongo.BlogPosts.Find(b => b.Id == id).FirstOrDefaultAsync(ct);

    public async Task<List<BlogPost>> GetAllAsync(int? churchId = null, bool onlyPublished = true, CancellationToken ct = default)
    {
        var filter = global::MongoDB.Driver.Builders<BlogPost>.Filter.Empty;

        if (onlyPublished)
            filter &= global::MongoDB.Driver.Builders<BlogPost>.Filter.Eq(b => b.Publication.IsPublished, true);

        if (churchId.HasValue)
            filter &= global::MongoDB.Driver.Builders<BlogPost>.Filter.Eq(b => b.ChurchId, churchId.Value);

        return await _mongo.BlogPosts
            .Find(filter)
            .SortByDescending(b => b.Publication.PublishedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(BlogPost post, CancellationToken ct = default) =>
        await _mongo.BlogPosts.InsertOneAsync(post, cancellationToken: ct);

    public async Task UpdateAsync(BlogPost post, CancellationToken ct = default) =>
        await _mongo.BlogPosts.ReplaceOneAsync(b => b.Id == post.Id, post, cancellationToken: ct);

    public async Task DeleteAsync(string id, CancellationToken ct = default) =>
        await _mongo.BlogPosts.DeleteOneAsync(b => b.Id == id, ct);
}
