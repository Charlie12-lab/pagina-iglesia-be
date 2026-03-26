using IglesiaNet.Domain.Churches;
using IglesiaNet.Domain.Events;
using IglesiaNet.Domain.Shared;

namespace IglesiaNet.Application.Events;

public class EventAppService
{
    private readonly IEventRepository _events;
    private readonly IChurchRepository _churches;

    public EventAppService(IEventRepository events, IChurchRepository churches)
    {
        _events = events;
        _churches = churches;
    }

    public async Task<List<EventDto>> GetAllAsync(
        int? churchId = null, DateTime? from = null, DateTime? to = null,
        CancellationToken ct = default)
    {
        var events = await _events.GetAllAsync(churchId, from, to, ct);
        var churchIds = events.Select(e => e.ChurchId).Distinct().ToList();
        var churches = await GetChurchMapAsync(churchIds, ct);

        return events.Select(e =>
        {
            churches.TryGetValue(e.ChurchId, out var church);
            return EventDto.From(e, church?.Name ?? "Iglesia", church?.LogoUrl);
        }).ToList();
    }

    public async Task<EventDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var ev = await _events.GetByIdAsync(id, ct);
        if (ev is null) return null;
        var church = await _churches.GetByIdAsync(ev.ChurchId, ct);
        return EventDto.From(ev, church?.Name ?? "Iglesia", church?.LogoUrl);
    }

    public async Task<EventDto> CreateAsync(CreateEventRequest request, CancellationToken ct = default)
    {
        var church = await _churches.GetByIdAsync(request.ChurchId, ct)
            ?? throw new DomainException("La iglesia especificada no existe");

        var ev = Event.Create(
            request.Title, request.Description,
            request.StartDate, request.EndDate,
            request.AllowsRegistration, request.MaxAttendees,
            request.Location, request.ImageUrl,
            request.IsPublished, request.ChurchId,
            request.EventType, request.Modality);

        await _events.AddAsync(ev, ct);
        await _events.SaveChangesAsync(ct);
        return EventDto.From(ev, church.Name, church.LogoUrl);
    }

    public async Task<EventDto?> UpdateAsync(int id, UpdateEventRequest request, CancellationToken ct = default)
    {
        var ev = await _events.GetByIdAsync(id, ct);
        if (ev is null) return null;

        ev.Update(request.Title, request.Description,
            request.StartDate, request.EndDate,
            request.AllowsRegistration, request.MaxAttendees,
            request.Location, request.ImageUrl, request.IsPublished,
            request.EventType, request.Modality);

        await _events.UpdateAsync(ev, ct);
        await _events.SaveChangesAsync(ct);

        var church = await _churches.GetByIdAsync(ev.ChurchId, ct);
        return EventDto.From(ev, church?.Name ?? "Iglesia", church?.LogoUrl);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var ev = await _events.GetByIdAsync(id, ct);
        if (ev is null) return false;
        await _events.DeleteAsync(id, ct);
        await _events.SaveChangesAsync(ct);
        return true;
    }

    public async Task<EventRegistrationDto> RegisterAsync(
        int eventId, EventRegistrationRequest request, CancellationToken ct = default)
    {
        var ev = await _events.GetByIdAsync(eventId, ct)
            ?? throw new DomainException("El evento no existe");

        // La lógica de validación vive en el Aggregate
        var registration = ev.Register(request.FullName, request.Email, request.Phone, request.Notes);

        await _events.UpdateAsync(ev, ct);
        await _events.SaveChangesAsync(ct);
        return EventRegistrationDto.From(registration);
    }

    public async Task<List<EventRegistrationDto>> GetRegistrationsAsync(int eventId, CancellationToken ct = default)
    {
        var registrations = await _events.GetRegistrationsByEventAsync(eventId, ct);
        return registrations.Select(EventRegistrationDto.From).ToList();
    }

    private async Task<Dictionary<int, Church>> GetChurchMapAsync(List<int> ids, CancellationToken ct)
    {
        var result = new Dictionary<int, Church>();
        foreach (var id in ids)
        {
            var church = await _churches.GetByIdAsync(id, ct);
            if (church is not null) result[id] = church;
        }
        return result;
    }
}
