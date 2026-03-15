using IglesiaNet.Domain.Shared;

namespace IglesiaNet.Domain.Events;

public class Event : Entity<int>
{
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public EventSchedule Schedule { get; private set; }
    public EventCapacity Capacity { get; private set; }
    public string? Location { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsPublished { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int ChurchId { get; private set; }

    private readonly List<EventRegistration> _registrations = new();
    public IReadOnlyCollection<EventRegistration> Registrations => _registrations.AsReadOnly();

    // Para EF Core
    private Event() : base()
    {
        Title = string.Empty;
        Schedule = null!;
        Capacity = null!;
    }

    private Event(
        string title, string? description, EventSchedule schedule,
        EventCapacity capacity, string? location, string? imageUrl,
        bool isPublished, int churchId) : base()
    {
        Title = title;
        Description = description;
        Schedule = schedule;
        Capacity = capacity;
        Location = location;
        ImageUrl = imageUrl;
        IsPublished = isPublished;
        ChurchId = churchId;
        CreatedAt = DateTime.UtcNow;
    }

    public static Event Create(
        string title, string? description, DateTime startDate, DateTime? endDate,
        bool allowsRegistration, int? maxAttendees,
        string? location, string? imageUrl, bool isPublished, int churchId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("El título del evento es requerido");

        var schedule = EventSchedule.Create(startDate, endDate);
        var capacity = EventCapacity.Create(allowsRegistration, maxAttendees);

        return new Event(title.Trim(), description?.Trim(), schedule, capacity,
            location?.Trim(), imageUrl?.Trim(), isPublished, churchId);
    }

    public void Update(
        string? title, string? description, DateTime? startDate, DateTime? endDate,
        bool? allowsRegistration, int? maxAttendees,
        string? location, string? imageUrl, bool? isPublished)
    {
        if (title is not null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("El título no puede estar vacío");
            Title = title.Trim();
        }

        if (description is not null) Description = description.Trim();
        if (location is not null) Location = location.Trim();
        if (imageUrl is not null) ImageUrl = imageUrl.Trim();
        if (isPublished.HasValue) IsPublished = isPublished.Value;

        if (startDate.HasValue || endDate.HasValue)
        {
            Schedule = EventSchedule.Create(
                startDate ?? Schedule.StartDate,
                endDate ?? Schedule.EndDate);
        }

        if (allowsRegistration.HasValue || maxAttendees.HasValue)
        {
            Capacity = EventCapacity.Create(
                allowsRegistration ?? Capacity.AllowsRegistration,
                maxAttendees ?? Capacity.MaxAttendees);
        }
    }

    public void Publish() => IsPublished = true;
    public void Unpublish() => IsPublished = false;

    // Lógica de dominio: la validación vive aquí, no en un servicio
    public EventRegistration Register(string fullName, string email, string? phone, string? notes)
    {
        if (!Capacity.AllowsRegistration)
            throw new DomainException("Este evento no acepta inscripciones");

        if (!Capacity.CanAcceptRegistration(_registrations.Count))
            throw new DomainException("El evento ha alcanzado su capacidad máxima");

        var registration = new EventRegistration(Id, fullName, email, phone, notes);
        _registrations.Add(registration);
        return registration;
    }
}
