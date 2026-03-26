using IglesiaNet.Domain.Blogs;
using IglesiaNet.Domain.Churches;
using IglesiaNet.Domain.Shared;

namespace IglesiaNet.Application.Blogs;

public class BlogAppService
{
    private readonly IBlogPostRepository _blogs;
    private readonly IChurchRepository _churches;

    public BlogAppService(IBlogPostRepository blogs, IChurchRepository churches)
    {
        _blogs = blogs;
        _churches = churches;
    }

    public async Task<List<BlogPostSummaryDto>> GetAllAsync(
        int? churchId = null, bool onlyPublished = true, CancellationToken ct = default)
    {
        var posts = await _blogs.GetAllAsync(churchId, onlyPublished, ct);
        return posts.Select(BlogPostSummaryDto.From).ToList();
    }

    public async Task<BlogPostDto?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var post = await _blogs.GetByIdAsync(id, ct);
        return post is null ? null : BlogPostDto.From(post);
    }

    public async Task<BlogPostDto> CreateAsync(CreateBlogPostRequest request, CancellationToken ct = default)
    {
        var church = await _churches.GetByIdAsync(request.ChurchId, ct)
            ?? throw new DomainException("La iglesia especificada no existe");

        var post = BlogPost.Create(
            request.Title, request.Content, request.Excerpt,
            request.Author, request.ChurchId, church.Name,
            request.CoverImageUrl, request.ImageUrls, request.Tags,
            request.IsPublished, request.Category);

        await _blogs.AddAsync(post, ct);
        return BlogPostDto.From(post);
    }

    public async Task<BlogPostDto?> UpdateAsync(string id, UpdateBlogPostRequest request, CancellationToken ct = default)
    {
        var post = await _blogs.GetByIdAsync(id, ct);
        if (post is null) return null;

        post.Update(request.Title, request.Content, request.Excerpt,
            request.Author, request.CoverImageUrl, request.ImageUrls,
            request.Tags, request.IsPublished, request.Category);

        await _blogs.UpdateAsync(post, ct);
        return BlogPostDto.From(post);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var post = await _blogs.GetByIdAsync(id, ct);
        if (post is null) return false;
        await _blogs.DeleteAsync(id, ct);
        return true;
    }
}
