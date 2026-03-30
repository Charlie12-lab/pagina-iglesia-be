using IglesiaNet.Domain.Blogs;
using Microsoft.EntityFrameworkCore;

namespace IglesiaNet.Infrastructure.Persistence.SQL.Repositories;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly AppDbContext _db;
    public BlogPostRepository(AppDbContext db) => _db = db;

    public async Task<BlogPost?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await _db.BlogPosts.FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<List<BlogPost>> GetAllAsync(
        int? churchId = null, bool onlyPublished = true, CancellationToken ct = default)
    {
        var q = _db.BlogPosts.AsQueryable();
        if (onlyPublished) q = q.Where(b => b.Publication.IsPublished);
        if (churchId.HasValue) q = q.Where(b => b.ChurchId == churchId.Value);
        return await q.OrderByDescending(b => b.Publication.PublishedAt)
                      .ThenByDescending(b => b.CreatedAt)
                      .ToListAsync(ct);
    }

    public async Task AddAsync(BlogPost post, CancellationToken ct = default)
    {
        await _db.BlogPosts.AddAsync(post, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(BlogPost post, CancellationToken ct = default)
    {
        _db.BlogPosts.Update(post);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        var post = await _db.BlogPosts.FindAsync(new object[] { id }, ct);
        if (post is not null)
        {
            _db.BlogPosts.Remove(post);
            await _db.SaveChangesAsync(ct);
        }
    }
}
