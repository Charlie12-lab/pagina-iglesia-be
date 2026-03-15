using IglesiaNet.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace IglesiaNet.Infrastructure.Persistence.SQL.Repositories;

public class EventRepository : IEventRepository
{
    private readonly AppDbContext _db;
    public EventRepository(AppDbContext db) => _db = db;

    public Task<Event?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Events
           .Include(e => e.Registrations)
           .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<List<Event>> GetAllAsync(
        int? churchId = null, DateTime? from = null, DateTime? to = null,
        CancellationToken ct = default)
    {
        var query = _db.Events
            .Include(e => e.Registrations)
            .Where(e => e.IsPublished)
            .AsQueryable();

        if (churchId.HasValue)
            query = query.Where(e => e.ChurchId == churchId.Value);

        if (from.HasValue)
            query = query.Where(e => e.Schedule.StartDate >= from.Value);

        if (to.HasValue)
            query = query.Where(e => e.Schedule.StartDate <= to.Value);

        return await query.OrderBy(e => e.Schedule.StartDate).ToListAsync(ct);
    }

    public async Task AddAsync(Event @event, CancellationToken ct = default) =>
        await _db.Events.AddAsync(@event, ct);

    public Task UpdateAsync(Event @event, CancellationToken ct = default)
    {
        _db.Events.Update(@event);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var ev = await _db.Events.FindAsync(new object[] { id }, ct);
        if (ev is not null) _db.Events.Remove(ev);
    }

    public Task<List<EventRegistration>> GetRegistrationsByEventAsync(int eventId, CancellationToken ct = default) =>
        _db.EventRegistrations
           .Where(r => r.EventId == eventId)
           .OrderBy(r => r.RegisteredAt)
           .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}
