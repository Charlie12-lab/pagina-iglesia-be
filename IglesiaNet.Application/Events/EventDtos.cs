using System.ComponentModel.DataAnnotations;
using IglesiaNet.Domain.Events;

namespace IglesiaNet.Application.Events;

public record EventDto(
    int Id, string Title, string? Description,
    DateTime StartDate, DateTime? EndDate, string? Location, string? ImageUrl,
    bool AllowsRegistration, int? MaxAttendees, int CurrentAttendees,
    bool IsPublished, int ChurchId, string ChurchName, string? ChurchLogoUrl,
    string? EventType, string? Modality, decimal? Price
)
{
    public static EventDto From(Event e, string churchName, string? churchLogoUrl) => new(
        e.Id, e.Title, e.Description,
        e.Schedule.StartDate, e.Schedule.EndDate, e.Location, e.ImageUrl,
        e.Capacity.AllowsRegistration, e.Capacity.MaxAttendees, e.Registrations.Count,
        e.IsPublished, e.ChurchId, churchName, churchLogoUrl,
        e.EventType, e.Modality, e.Price);
}

public record CreateEventRequest(
    [Required, MaxLength(300)] string Title,
    string? Description,
    [Required] DateTime StartDate,
    DateTime? EndDate,
    bool AllowsRegistration,
    int? MaxAttendees,
    string? Location,
    string? ImageUrl,
    bool IsPublished,
    [Required] int ChurchId,
    string? EventType,
    string? Modality,
    decimal? Price
);

public record UpdateEventRequest(
    string? Title, string? Description,
    DateTime? StartDate, DateTime? EndDate,
    bool? AllowsRegistration, int? MaxAttendees,
    string? Location, string? ImageUrl, bool? IsPublished,
    string? EventType, string? Modality, decimal? Price
);

// ── Inscripción individual ────────────────────────────────────────────────────
public record EventRegistrationRequest(
    [Required, MaxLength(200)] string FullName,
    string? Email,
    string? Phone,
    string? Notes,
    string? Church
);

// ── Inscripción grupal ────────────────────────────────────────────────────────
public record GroupMemberRequest(
    [Required, MaxLength(200)] string FullName,
    string? Email,
    string? Phone
);

public record GroupRegistrationRequest(
    [Required, MaxLength(200)] string Responsible,
    [Required] string Email,
    string? Phone,
    string? Church,
    [Required] List<GroupMemberRequest> Members
);

// ── DTOs de salida ────────────────────────────────────────────────────────────
public record EventRegistrationDto(
    int Id, string FullName, string? Email,
    string? Phone, string? Notes, string? Church,
    string? VoucherPath, string? GroupId, DateTime RegisteredAt
)
{
    public static EventRegistrationDto From(EventRegistration r) => new(
        r.Id, r.FullName, r.Email, r.Phone, r.Notes,
        r.Church, r.VoucherPath, r.GroupId, r.RegisteredAt);
}
