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
    // Nuevos campos
    public string? EventType { get; private set; }  // Vigilia|Conferencia|Campamento|Campaña|Aniversario|Otros
    public string? Modality { get; private set; }   // Presencial|Online|Híbrido
    public decimal? Price { get; private set; }     // null o 0 = gratuito

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
        bool isPublished, int churchId,
        string? eventType, string? modality, decimal? price) : base()
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
        EventType = eventType;
        Modality = modality;
        Price = price is > 0 ? price : null;
    }

    public static Event Create(
        string title, string? description, DateTime startDate, DateTime? endDate,
        bool allowsRegistration, int? maxAttendees,
        string? location, string? imageUrl, bool isPublished, int churchId,
        string? eventType = null, string? modality = null, decimal? price = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("El título del evento es requerido");

        var schedule = EventSchedule.Create(startDate, endDate);
        var capacity = EventCapacity.Create(allowsRegistration, maxAttendees);

        return new Event(title.Trim(), description?.Trim(), schedule, capacity,
            location?.Trim(), imageUrl?.Trim(), isPublished, churchId,
            eventType?.Trim(), modality?.Trim(), price);
    }

    public void Update(
        string? title, string? description, DateTime? startDate, DateTime? endDate,
        bool? allowsRegistration, int? maxAttendees,
        string? location, string? imageUrl, bool? isPublished,
        string? eventType = null, string? modality = null, decimal? price = null)
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
        if (eventType is not null) EventType = eventType.Trim();
        if (modality is not null) Modality = modality.Trim();
        if (price.HasValue) Price = price.Value > 0 ? price.Value : null;

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

    // Inscripción individual
    public EventRegistration Register(
        string fullName, string? email, string? phone, string? notes,
        string? church = null, string? voucherPath = null)
    {
        if (!Capacity.AllowsRegistration)
            throw new DomainException("Este evento no acepta inscripciones");

        if (!Capacity.CanAcceptRegistration(_registrations.Count))
            throw new DomainException("El evento ha alcanzado su capacidad máxima");

        var registration = EventRegistration.CreateIndividual(Id, fullName, email, phone, notes, church, voucherPath);
        _registrations.Add(registration);
        return registration;
    }

    // Inscripción grupal — valida capacidad para el lote completo
    public List<EventRegistration> RegisterGroup(
        IEnumerable<(string FullName, string? Email, string? Phone)> members,
        string? church, string? voucherPath)
    {
        if (!Capacity.AllowsRegistration)
            throw new DomainException("Este evento no acepta inscripciones");

        var memberList = members.ToList();
        if (!Capacity.CanAcceptRegistration(_registrations.Count + memberList.Count - 1))
            throw new DomainException("No hay suficientes cupos para el grupo");

        var groupId = Guid.NewGuid().ToString("N");
        var registrations = new List<EventRegistration>();

        foreach (var (fullName, email, phone) in memberList)
        {
            var reg = EventRegistration.CreateGroupMember(Id, fullName, email, phone, church, voucherPath, groupId);
            _registrations.Add(reg);
            registrations.Add(reg);
        }

        return registrations;
    }
}
