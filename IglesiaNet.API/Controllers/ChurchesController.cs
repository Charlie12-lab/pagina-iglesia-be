using IglesiaNet.API.Data;
using IglesiaNet.API.DTOs;
using IglesiaNet.API.Models.SQL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IglesiaNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChurchesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ChurchesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var churches = await _db.Churches
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new ChurchDto(c.Id, c.Name, c.Address, c.City, c.Phone,
                c.Email, c.Description, c.LogoUrl, c.WebsiteUrl, c.IsActive))
            .ToListAsync();

        return Ok(churches);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _db.Churches.FindAsync(id);
        if (c is null) return NotFound();

        return Ok(new ChurchDto(c.Id, c.Name, c.Address, c.City, c.Phone,
            c.Email, c.Description, c.LogoUrl, c.WebsiteUrl, c.IsActive));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateChurchRequest request)
    {
        var church = new Church
        {
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            Phone = request.Phone,
            Email = request.Email,
            Description = request.Description,
            LogoUrl = request.LogoUrl,
            WebsiteUrl = request.WebsiteUrl
        };

        _db.Churches.Add(church);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = church.Id },
            new ChurchDto(church.Id, church.Name, church.Address, church.City, church.Phone,
                church.Email, church.Description, church.LogoUrl, church.WebsiteUrl, church.IsActive));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateChurchRequest request)
    {
        var church = await _db.Churches.FindAsync(id);
        if (church is null) return NotFound();

        if (request.Name is not null) church.Name = request.Name;
        if (request.Address is not null) church.Address = request.Address;
        if (request.City is not null) church.City = request.City;
        if (request.Phone is not null) church.Phone = request.Phone;
        if (request.Email is not null) church.Email = request.Email;
        if (request.Description is not null) church.Description = request.Description;
        if (request.LogoUrl is not null) church.LogoUrl = request.LogoUrl;
        if (request.WebsiteUrl is not null) church.WebsiteUrl = request.WebsiteUrl;
        if (request.IsActive.HasValue) church.IsActive = request.IsActive.Value;

        await _db.SaveChangesAsync();
        return Ok(new ChurchDto(church.Id, church.Name, church.Address, church.City, church.Phone,
            church.Email, church.Description, church.LogoUrl, church.WebsiteUrl, church.IsActive));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var church = await _db.Churches.FindAsync(id);
        if (church is null) return NotFound();
        church.IsActive = false; // Soft delete
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
