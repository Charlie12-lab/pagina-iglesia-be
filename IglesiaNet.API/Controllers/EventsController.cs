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
    public EventsController(EventAppService events) => _events = events;

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

    [HttpPost("{id}/register")]
    public async Task<IActionResult> Register(int id, [FromBody] EventRegistrationRequest request, CancellationToken ct)
    {
        try
        {
            var reg = await _events.RegisterAsync(id, request, ct);
            return Ok(reg);
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

    private bool IsAuthorizedForChurch(int churchId)
    {
        if (User.IsInRole("SuperAdmin")) return true;
        var claim = User.FindFirst("churchId")?.Value;
        return int.TryParse(claim, out var userChurchId) && userChurchId == churchId;
    }
}
