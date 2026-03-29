using IglesiaNet.Domain.Shared;

namespace IglesiaNet.Domain.Events;

public class EventRegistration : Entity<int>
{
    public string FullName { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Notes { get; private set; }
    public string? Church { get; private set; }      // Iglesia de procedencia
    public string? VoucherPath { get; private set; } // Ruta del comprobante de pago
    public string? GroupId { get; private set; }     // Correlaciona inscripciones grupales
    public DateTime RegisteredAt { get; private set; }
    public int EventId { get; private set; }

    // Para EF Core
    private EventRegistration() : base()
    {
        FullName = string.Empty;
    }

    private EventRegistration(
        int eventId, string fullName, string? email,
        string? phone, string? notes, string? church,
        string? voucherPath, string? groupId) : base()
    {
        EventId = eventId;
        FullName = fullName.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLowerInvariant();
        Phone = phone?.Trim();
        Notes = notes?.Trim();
        Church = church?.Trim();
        VoucherPath = voucherPath;
        GroupId = groupId;
        RegisteredAt = DateTime.UtcNow;
    }

    // Inscripción individual — email requerido y validado
    internal static EventRegistration CreateIndividual(
        int eventId, string fullName, string email,
        string? phone, string? notes, string? church, string? voucherPath)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("El nombre completo es requerido");
        Shared.Email.Create(email);

        return new EventRegistration(eventId, fullName, email, phone, notes, church, voucherPath, null);
    }

    // Miembro de inscripción grupal — email opcional
    internal static EventRegistration CreateGroupMember(
        int eventId, string fullName, string? email,
        string? phone, string? church, string? voucherPath, string groupId)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("El nombre completo es requerido");
        if (!string.IsNullOrWhiteSpace(email))
            Shared.Email.Create(email);

        return new EventRegistration(eventId, fullName, email, phone, null, church, voucherPath, groupId);
    }
}
