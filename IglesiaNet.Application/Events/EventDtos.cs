using System.ComponentModel.DataAnnotations;
using IglesiaNet.Domain.Events;

namespace IglesiaNet.Application.Events;

public record EventDto(
    int Id, string Title, string? Description,
    DateTime StartDate, DateTime? EndDate, string? Location, string? ImageUrl,
    bool AllowsRegistration, int? MaxAttendees, int CurrentAttendees,
    bool IsPublished, int ChurchId, string ChurchName, string? ChurchLogoUrl
)
{
    public static EventDto From(Event e, string churchName, string? churchLogoUrl) => new(
        e.Id, e.Title, e.Description,
        e.Schedule.StartDate, e.Schedule.EndDate, e.Location, e.ImageUrl,
        e.Capacity.AllowsRegistration, e.Capacity.MaxAttendees, e.Registrations.Count,
        e.IsPublished, e.ChurchId, churchName, churchLogoUrl);
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
    [Required] int ChurchId
);

public record UpdateEventRequest(
    string? Title, string? Description,
    DateTime? StartDate, DateTime? EndDate,
    bool? AllowsRegistration, int? MaxAttendees,
    string? Location, string? ImageUrl, bool? IsPublished
);

public record EventRegistrationRequest(
    [Required, MaxLength(200)] string FullName,
    [Required] string Email,
    string? Phone,
    string? Notes
);

public record EventRegistrationDto(
    int Id, string FullName, string Email,
    string? Phone, string? Notes, DateTime RegisteredAt
)
{
    public static EventRegistrationDto From(EventRegistration r) => new(
        r.Id, r.FullName, r.Email, r.Phone, r.Notes, r.RegisteredAt);
}
