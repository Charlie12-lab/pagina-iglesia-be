using System.ComponentModel.DataAnnotations;

namespace IglesiaNet.API.DTOs;

public record EventDto(
    int Id,
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime? EndDate,
    string? Location,
    string? ImageUrl,
    bool AllowsRegistration,
    int? MaxAttendees,
    int CurrentAttendees,
    bool IsPublished,
    int ChurchId,
    string ChurchName,
    string? ChurchLogoUrl
);

public record CreateEventRequest(
    [Required, MaxLength(300)] string Title,
    string? Description,
    [Required] DateTime StartDate,
    DateTime? EndDate,
    string? Location,
    string? ImageUrl,
    bool AllowsRegistration,
    int? MaxAttendees,
    bool IsPublished,
    [Required] int ChurchId
);

public record UpdateEventRequest(
    string? Title,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate,
    string? Location,
    string? ImageUrl,
    bool? AllowsRegistration,
    int? MaxAttendees,
    bool? IsPublished
);

public record EventRegistrationRequest(
    [Required, MaxLength(200)] string FullName,
    [Required, EmailAddress] string Email,
    string? Phone,
    string? Notes
);

public record EventRegistrationDto(
    int Id,
    string FullName,
    string Email,
    string? Phone,
    string? Notes,
    DateTime RegisteredAt
);
