using IglesiaNet.API.Data;
using IglesiaNet.API.DTOs;
using IglesiaNet.API.Models.MongoDB;
using MongoDB.Driver;

namespace IglesiaNet.API.Services;

public class BlogService
{
    private readonly MongoDbContext _mongo;

    public BlogService(MongoDbContext mongo) => _mongo = mongo;

    public async Task<List<BlogPostSummaryDto>> GetAllAsync(int? churchId = null, bool onlyPublished = true)
    {
        var filter = Builders<BlogPost>.Filter.Empty;

        if (onlyPublished)
            filter &= Builders<BlogPost>.Filter.Eq(b => b.IsPublished, true);

        if (churchId.HasValue)
            filter &= Builders<BlogPost>.Filter.Eq(b => b.ChurchId, churchId.Value);

        var posts = await _mongo.BlogPosts
            .Find(filter)
            .SortByDescending(b => b.PublishedAt)
            .ToListAsync();

        return posts.Select(MapToSummary).ToList();
    }

    public async Task<BlogPostDto?> GetByIdAsync(string id)
    {
        var post = await _mongo.BlogPosts.Find(b => b.Id == id).FirstOrDefaultAsync();
        return post is null ? null : MapToDto(post);
    }

    public async Task<BlogPostDto> CreateAsync(CreateBlogPostRequest request)
    {
        var post = new BlogPost
        {
            Title = request.Title,
            Content = request.Content,
            Excerpt = request.Excerpt,
            Author = request.Author,
            ChurchId = request.ChurchId,
            ChurchName = string.Empty, // Se resuelve en controller
            CoverImageUrl = request.CoverImageUrl,
            ImageUrls = request.ImageUrls ?? new List<string>(),
            Tags = request.Tags ?? new List<string>(),
            IsPublished = request.IsPublished,
            PublishedAt = request.IsPublished ? DateTime.UtcNow : null
        };

        await _mongo.BlogPosts.InsertOneAsync(post);
        return MapToDto(post);
    }

    public async Task<BlogPostDto?> UpdateAsync(string id, UpdateBlogPostRequest request, string churchName)
    {
        var post = await _mongo.BlogPosts.Find(b => b.Id == id).FirstOrDefaultAsync();
        if (post is null) return null;

        var update = Builders<BlogPost>.Update
            .Set(b => b.UpdatedAt, DateTime.UtcNow);

        if (request.Title is not null) update = update.Set(b => b.Title, request.Title);
        if (request.Content is not null) update = update.Set(b => b.Content, request.Content);
        if (request.Excerpt is not null) update = update.Set(b => b.Excerpt, request.Excerpt);
        if (request.Author is not null) update = update.Set(b => b.Author, request.Author);
        if (request.CoverImageUrl is not null) update = update.Set(b => b.CoverImageUrl, request.CoverImageUrl);
        if (request.ImageUrls is not null) update = update.Set(b => b.ImageUrls, request.ImageUrls);
        if (request.Tags is not null) update = update.Set(b => b.Tags, request.Tags);
        if (request.IsPublished.HasValue)
        {
            update = update.Set(b => b.IsPublished, request.IsPublished.Value);
            if (request.IsPublished.Value && post.PublishedAt is null)
                update = update.Set(b => b.PublishedAt, DateTime.UtcNow);
        }

        await _mongo.BlogPosts.UpdateOneAsync(b => b.Id == id, update);
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _mongo.BlogPosts.DeleteOneAsync(b => b.Id == id);
        return result.DeletedCount > 0;
    }

    private static BlogPostDto MapToDto(BlogPost b) => new(
        b.Id, b.Title, b.Content, b.Excerpt, b.Author,
        b.ChurchId, b.ChurchName, b.CoverImageUrl,
        b.ImageUrls, b.Tags, b.IsPublished, b.PublishedAt, b.CreatedAt
    );

    private static BlogPostSummaryDto MapToSummary(BlogPost b) => new(
        b.Id, b.Title, b.Excerpt, b.Author,
        b.ChurchId, b.ChurchName, b.CoverImageUrl,
        b.Tags, b.PublishedAt
    );
}
