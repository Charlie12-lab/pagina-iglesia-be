using IglesiaNet.Domain.Shared;

namespace IglesiaNet.Domain.Events;

public sealed class EventSchedule : ValueObject
{
    public DateTime StartDate { get; }
    public DateTime? EndDate { get; }

    private EventSchedule(DateTime startDate, DateTime? endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public static EventSchedule Create(DateTime startDate, DateTime? endDate = null)
    {
        if (endDate.HasValue && endDate.Value < startDate)
            throw new DomainException("La fecha de fin no puede ser anterior a la fecha de inicio");
        return new EventSchedule(startDate, endDate);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }
}
