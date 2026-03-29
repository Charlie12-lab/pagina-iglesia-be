using System.Text.Json;
using IglesiaNet.Application.Events;
using IglesiaNet.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly EventAppService _events;
    private readonly IWebHostEnvironment _env;

    public EventsController(EventAppService events, IWebHostEnvironment env)
    {
        _events = events;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? churchId, [FromQuery] DateTime? from,
        [FromQuery] DateTime? to, CancellationToken ct) =>
        Ok(await _events.GetAllAsync(churchId, from, to, ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var dto = await _events.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateEventRequest request, CancellationToken ct)
    {
        if (!IsAuthorizedForChurch(request.ChurchId)) return Forbid();
        try
        {
            var dto = await _events.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (DomainException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEventRequest request, CancellationToken ct)
    {
        var existing = await _events.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();
        if (!IsAuthorizedForChurch(existing.ChurchId)) return Forbid();
        try
        {
            var dto = await _events.UpdateAsync(id, request, ct);
            return dto is null ? NotFound() : Ok(dto);
        }
        catch (DomainException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var existing = await _events.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();
        if (!IsAuthorizedForChurch(existing.ChurchId)) return Forbid();
        await _events.DeleteAsync(id, ct);
        return NoContent();
    }

    // ── Inscripción individual (multipart/form-data) ──────────────────────────
    [HttpPost("{id}/register")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Register(
        int id,
        [FromForm] string fullName,
        [FromForm] string email,
        [FromForm] string? phone,
        [FromForm] string? notes,
        [FromForm] string? church,
        IFormFile? voucher,
        CancellationToken ct)
    {
        var voucherPath = voucher is not null ? await SaveVoucherAsync(voucher, id) : null;
        var request = new EventRegistrationRequest(fullName, email, phone, notes, church);
        try
        {
            var reg = await _events.RegisterAsync(id, request, voucherPath, ct);
            return Ok(reg);
        }
        catch (DomainException ex) { return BadRequest(new { message = ex.Message }); }
    }

    // ── Inscripción grupal (multipart/form-data) ──────────────────────────────
    [HttpPost("{id}/register-group")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> RegisterGroup(
        int id,
        [FromForm] string responsible,
        [FromForm] string email,
        [FromForm] string? phone,
        [FromForm] string? church,
        [FromForm] string membersJson,   // JSON string de la lista de miembros
        IFormFile? voucher,
        CancellationToken ct)
    {
        List<GroupMemberRequest> members;
        try
        {
            members = JsonSerializer.Deserialize<List<GroupMemberRequest>>(
                membersJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<GroupMemberRequest>();
        }
        catch
        {
            return BadRequest(new { message = "El formato de los miembros no es válido" });
        }

        if (members.Count == 0)
            return BadRequest(new { message = "El grupo debe tener al menos un miembro" });

        var voucherPath = voucher is not null ? await SaveVoucherAsync(voucher, id) : null;
        var request = new GroupRegistrationRequest(responsible, email, phone, church, members);

        try
        {
            var regs = await _events.RegisterGroupAsync(id, request, voucherPath, ct);
            return Ok(regs);
        }
        catch (DomainException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("{id}/registrations")]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> GetRegistrations(int id, CancellationToken ct)
    {
        var existing = await _events.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();
        if (!IsAuthorizedForChurch(existing.ChurchId)) return Forbid();
        return Ok(await _events.GetRegistrationsAsync(id, ct));
    }

    // ── Upload imagen del evento ──────────────────────────────────────────────
    [HttpPost("upload-image")]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage(IFormFile image, CancellationToken ct)
    {
        if (image is null)
            return BadRequest(new { message = "No se proporcionó imagen" });

        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext))
            return BadRequest(new { message = "Formato no permitido. Usa JPG, PNG, WEBP o GIF." });

        var uploadsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "events");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"ev_{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadsDir, fileName);

        await using var stream = System.IO.File.Create(fullPath);
        await image.CopyToAsync(stream, ct);

        return Ok(new { url = $"/uploads/events/{fileName}" });
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private bool IsAuthorizedForChurch(int churchId)
    {
        if (User.IsInRole("SuperAdmin")) return true;
        var claim = User.FindFirst("churchId")?.Value;
        return int.TryParse(claim, out var userChurchId) && userChurchId == churchId;
    }

    private async Task<string> SaveVoucherAsync(IFormFile file, int eventId)
    {
        var uploadsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "vouchers");
        Directory.CreateDirectory(uploadsDir);

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"ev{eventId}_{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadsDir, fileName);

        await using var stream = System.IO.File.Create(fullPath);
        await file.CopyToAsync(stream);

        return $"/uploads/vouchers/{fileName}";
    }
}
