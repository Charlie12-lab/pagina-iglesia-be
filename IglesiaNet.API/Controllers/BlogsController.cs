using IglesiaNet.API.Data;
using IglesiaNet.API.DTOs;
using IglesiaNet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IglesiaNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogsController : ControllerBase
{
    private readonly BlogService _blogService;
    private readonly AppDbContext _db;

    public BlogsController(BlogService blogService, AppDbContext db)
    {
        _blogService = blogService;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? churchId)
    {
        var isAdmin = User.Identity?.IsAuthenticated == true;
        var posts = await _blogService.GetAllAsync(churchId, onlyPublished: !isAdmin);
        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var post = await _blogService.GetByIdAsync(id);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateBlogPostRequest request)
    {
        if (User.IsInRole("ChurchAdmin"))
        {
            var churchIdClaim = User.FindFirst("churchId")?.Value;
            if (!int.TryParse(churchIdClaim, out int adminChurchId) || adminChurchId != request.ChurchId)
                return Forbid();
        }

        var church = await _db.Churches.FindAsync(request.ChurchId);
        if (church is null) return BadRequest(new { message = "Iglesia no encontrada" });

        // Inyectamos el nombre de la iglesia para desnormalizar en MongoDB
        var requestWithChurch = request with { };
        var post = await _blogService.CreateAsync(requestWithChurch);

        // Actualizamos el nombre de la iglesia directamente
        var updateRequest = new UpdateBlogPostRequest(null, null, null, null, null, null, null, null);
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateBlogPostRequest request)
    {
        var existing = await _blogService.GetByIdAsync(id);
        if (existing is null) return NotFound();

        if (User.IsInRole("ChurchAdmin"))
        {
            var churchIdClaim = User.FindFirst("churchId")?.Value;
            if (!int.TryParse(churchIdClaim, out int adminChurchId) || adminChurchId != existing.ChurchId)
                return Forbid();
        }

        var church = await _db.Churches.FindAsync(existing.ChurchId);
        var post = await _blogService.UpdateAsync(id, request, church?.Name ?? "");
        return post is null ? NotFound() : Ok(post);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _blogService.GetByIdAsync(id);
        if (existing is null) return NotFound();

        if (User.IsInRole("ChurchAdmin"))
        {
            var churchIdClaim = User.FindFirst("churchId")?.Value;
            if (!int.TryParse(churchIdClaim, out int adminChurchId) || adminChurchId != existing.ChurchId)
                return Forbid();
        }

        await _blogService.DeleteAsync(id);
        return NoContent();
    }
}
