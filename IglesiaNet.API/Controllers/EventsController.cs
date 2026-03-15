using System.Security.Claims;
using IglesiaNet.API.DTOs;
using IglesiaNet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly EventService _eventService;

    public EventsController(EventService eventService) => _eventService = eventService;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? churchId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var events = await _eventService.GetAllAsync(churchId, from, to);
        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ev = await _eventService.GetByIdAsync(id);
        return ev is null ? NotFound() : Ok(ev);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
    {
        // ChurchAdmin solo puede crear eventos de su iglesia
        if (User.IsInRole("ChurchAdmin"))
        {
            var churchIdClaim = User.FindFirst("churchId")?.Value;
            if (!int.TryParse(churchIdClaim, out int adminChurchId) || adminChurchId != request.ChurchId)
                return Forbid();
        }

        var ev = await _eventService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = ev.Id }, ev);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEventRequest request)
    {
        var existing = await _eventService.GetByIdAsync(id);
        if (existing is null) return NotFound();

        if (User.IsInRole("ChurchAdmin"))
        {
            var churchIdClaim = User.FindFirst("churchId")?.Value;
            if (!int.TryParse(churchIdClaim, out int adminChurchId) || adminChurchId != existing.ChurchId)
                return Forbid();
        }

        var ev = await _eventService.UpdateAsync(id, request);
        return ev is null ? NotFound() : Ok(ev);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _eventService.GetByIdAsync(id);
        if (existing is null) return NotFound();

        if (User.IsInRole("ChurchAdmin"))
        {
            var churchIdClaim = User.FindFirst("churchId")?.Value;
            if (!int.TryParse(churchIdClaim, out int adminChurchId) || adminChurchId != existing.ChurchId)
                return Forbid();
        }

        await _eventService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/register")]
    public async Task<IActionResult> Register(int id, [FromBody] EventRegistrationRequest request)
    {
        try
        {
            var reg = await _eventService.RegisterAsync(id, request);
            if (reg is null) return BadRequest(new { message = "El evento no permite inscripciones" });
            return Ok(reg);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/registrations")]
    [Authorize(Roles = "SuperAdmin,ChurchAdmin")]
    public async Task<IActionResult> GetRegistrations(int id)
    {
        var existing = await _eventService.GetByIdAsync(id);
        if (existing is null) return NotFound();

        if (User.IsInRole("ChurchAdmin"))
        {
            var churchIdClaim = User.FindFirst("churchId")?.Value;
            if (!int.TryParse(churchIdClaim, out int adminChurchId) || adminChurchId != existing.ChurchId)
                return Forbid();
        }

        var registrations = await _eventService.GetRegistrationsAsync(id);
        return Ok(registrations);
    }
}
