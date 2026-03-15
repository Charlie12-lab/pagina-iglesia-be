namespace IglesiaNet.Domain.Events;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Event>> GetAllAsync(int? churchId = null, DateTime? from = null, DateTime? to = null, CancellationToken ct = default);
    Task AddAsync(Event @event, CancellationToken ct = default);
    Task UpdateAsync(Event @event, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<List<EventRegistration>> GetRegistrationsByEventAsync(int eventId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
