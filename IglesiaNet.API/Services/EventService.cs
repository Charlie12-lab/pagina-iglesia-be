using IglesiaNet.API.Data;
using IglesiaNet.API.DTOs;
using IglesiaNet.API.Models.SQL;
using Microsoft.EntityFrameworkCore;

namespace IglesiaNet.API.Services;

public class EventService
{
    private readonly AppDbContext _db;

    public EventService(AppDbContext db) => _db = db;

    public async Task<List<EventDto>> GetAllAsync(int? churchId = null, DateTime? from = null, DateTime? to = null)
    {
        var query = _db.Events
            .Include(e => e.Church)
            .Include(e => e.Registrations)
            .Where(e => e.IsPublished)
            .AsQueryable();

        if (churchId.HasValue)
            query = query.Where(e => e.ChurchId == churchId.Value);

        if (from.HasValue)
            query = query.Where(e => e.StartDate >= from.Value);

        if (to.HasValue)
            query = query.Where(e => e.StartDate <= to.Value);

        return await query
            .OrderBy(e => e.StartDate)
            .Select(e => MapToDto(e))
            .ToListAsync();
    }

    public async Task<EventDto?> GetByIdAsync(int id)
    {
        var e = await _db.Events
            .Include(e => e.Church)
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == id);

        return e is null ? null : MapToDto(e);
    }

    public async Task<EventDto> CreateAsync(CreateEventRequest request)
    {
        var ev = new Event
        {
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Location = request.Location,
            ImageUrl = request.ImageUrl,
            AllowsRegistration = request.AllowsRegistration,
            MaxAttendees = request.MaxAttendees,
            IsPublished = request.IsPublished,
            ChurchId = request.ChurchId
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(ev.Id))!;
    }

    public async Task<EventDto?> UpdateAsync(int id, UpdateEventRequest request)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev is null) return null;

        if (request.Title is not null) ev.Title = request.Title;
        if (request.Description is not null) ev.Description = request.Description;
        if (request.StartDate.HasValue) ev.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) ev.EndDate = request.EndDate.Value;
        if (request.Location is not null) ev.Location = request.Location;
        if (request.ImageUrl is not null) ev.ImageUrl = request.ImageUrl;
        if (request.AllowsRegistration.HasValue) ev.AllowsRegistration = request.AllowsRegistration.Value;
        if (request.MaxAttendees.HasValue) ev.MaxAttendees = request.MaxAttendees.Value;
        if (request.IsPublished.HasValue) ev.IsPublished = request.IsPublished.Value;

        await _db.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev is null) return false;
        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<EventRegistrationDto?> RegisterAsync(int eventId, EventRegistrationRequest request)
    {
        var ev = await _db.Events.Include(e => e.Registrations).FirstOrDefaultAsync(e => e.Id == eventId);
        if (ev is null || !ev.AllowsRegistration) return null;

        if (ev.MaxAttendees.HasValue && ev.Registrations.Count >= ev.MaxAttendees.Value)
            throw new InvalidOperationException("El evento ha alcanzado su capacidad máxima");

        var registration = new EventRegistration
        {
            EventId = eventId,
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Notes = request.Notes
        };

        _db.EventRegistrations.Add(registration);
        await _db.SaveChangesAsync();

        return new EventRegistrationDto(
            registration.Id,
            registration.FullName,
            registration.Email,
            registration.Phone,
            registration.Notes,
            registration.RegisteredAt
        );
    }

    public async Task<List<EventRegistrationDto>> GetRegistrationsAsync(int eventId)
    {
        return await _db.EventRegistrations
            .Where(r => r.EventId == eventId)
            .OrderBy(r => r.RegisteredAt)
            .Select(r => new EventRegistrationDto(r.Id, r.FullName, r.Email, r.Phone, r.Notes, r.RegisteredAt))
            .ToListAsync();
    }

    private static EventDto MapToDto(Event e) => new(
        e.Id, e.Title, e.Description, e.StartDate, e.EndDate,
        e.Location, e.ImageUrl, e.AllowsRegistration, e.MaxAttendees,
        e.Registrations.Count, e.IsPublished,
        e.ChurchId, e.Church.Name, e.Church.LogoUrl
    );
}
