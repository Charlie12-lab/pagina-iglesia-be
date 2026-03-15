namespace IglesiaNet.Domain.Blogs;

public interface IBlogPostRepository
{
    Task<BlogPost?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<List<BlogPost>> GetAllAsync(int? churchId = null, bool onlyPublished = true, CancellationToken ct = default);
    Task AddAsync(BlogPost post, CancellationToken ct = default);
    Task UpdateAsync(BlogPost post, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
