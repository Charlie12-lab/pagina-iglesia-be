using IglesiaNet.Domain.Shared;

namespace IglesiaNet.Domain.Events;

public sealed class EventCapacity : ValueObject
{
    public bool AllowsRegistration { get; }
    public int? MaxAttendees { get; }

    private EventCapacity(bool allowsRegistration, int? maxAttendees)
    {
        AllowsRegistration = allowsRegistration;
        MaxAttendees = maxAttendees;
    }

    public static EventCapacity Create(bool allowsRegistration, int? maxAttendees = null)
    {
        if (maxAttendees.HasValue && maxAttendees.Value < 1)
            throw new DomainException("El número máximo de asistentes debe ser mayor a cero");
        return new EventCapacity(allowsRegistration, maxAttendees);
    }

    public static EventCapacity NoRegistration() => new(false, null);

    public bool CanAcceptRegistration(int currentCount)
    {
        if (!AllowsRegistration) return false;
        if (!MaxAttendees.HasValue) return true;
        return currentCount < MaxAttendees.Value;
    }

    public int? SpotsLeft(int currentCount) =>
        MaxAttendees.HasValue ? MaxAttendees.Value - currentCount : null;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return AllowsRegistration;
        yield return MaxAttendees;
    }
}
