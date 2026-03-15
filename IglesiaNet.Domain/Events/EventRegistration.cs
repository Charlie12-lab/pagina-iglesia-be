using IglesiaNet.Domain.Shared;

namespace IglesiaNet.Domain.Events;

public class EventRegistration : Entity<int>
{
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Notes { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    public int EventId { get; private set; }

    // Para EF Core
    private EventRegistration() : base()
    {
        FullName = string.Empty;
        Email = string.Empty;
    }

    internal EventRegistration(int eventId, string fullName, string email, string? phone, string? notes)
        : base()
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("El nombre completo es requerido");

        Shared.Email.Create(email); // Valida formato

        EventId = eventId;
        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        Phone = phone?.Trim();
        Notes = notes?.Trim();
        RegisteredAt = DateTime.UtcNow;
    }
}
