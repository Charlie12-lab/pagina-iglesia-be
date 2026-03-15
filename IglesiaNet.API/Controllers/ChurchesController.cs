using IglesiaNet.Application.Churches;
using IglesiaNet.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChurchesController : ControllerBase
{
    private readonly ChurchAppService _churches;
    public ChurchesController(ChurchAppService churches) => _churches = churches;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await _churches.GetAllAsync(ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var dto = await _churches.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateChurchRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _churches.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (DomainException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateChurchRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _churches.UpdateAsync(id, request, ct);
            return dto is null ? NotFound() : Ok(dto);
        }
        catch (DomainException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await _churches.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
