using IglesiaNet.Application.Blogs;
using IglesiaNet.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogsController : ControllerBase
{
    private readonly BlogAppService _blogs;
    public BlogsController(BlogAppService blogs) => _blogs = blogs;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? churchId, CancellationToken ct)
    {
        var onlyPublished = User.Identity?.IsAuthenticated != true;
        return Ok(await _blogs.GetAllAsync(churchId, onlyPublished, ct));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var dto = await _blogs.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateBlogPostRequest request, CancellationToken ct)
    {
        if (!IsAuthorizedForChurch(request.ChurchId)) return Forbid();
        try
        {
            var dto = await _blogs.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (DomainException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateBlogPostRequest request, CancellationToken ct)
    {
        var existing = await _blogs.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();
        if (!IsAuthorizedForChurch(existing.ChurchId)) return Forbid();
        try
        {
            var dto = await _blogs.UpdateAsync(id, request, ct);
            return dto is null ? NotFound() : Ok(dto);
        }
        catch (DomainException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var existing = await _blogs.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();
        if (!IsAuthorizedForChurch(existing.ChurchId)) return Forbid();
        await _blogs.DeleteAsync(id, ct);
        return NoContent();
    }

    private bool IsAuthorizedForChurch(int churchId)
    {
        if (User.IsInRole("SuperAdmin")) return true;
        var claim = User.FindFirst("churchId")?.Value;
        return int.TryParse(claim, out var userChurchId) && userChurchId == churchId;
    }
}
