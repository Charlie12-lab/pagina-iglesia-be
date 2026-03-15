using System.ComponentModel.DataAnnotations;

namespace IglesiaNet.API.DTOs;

public record ChurchDto(
    int Id,
    string Name,
    string? Address,
    string? City,
    string? Phone,
    string? Email,
    string? Description,
    string? LogoUrl,
    string? WebsiteUrl,
    bool IsActive
);

public record CreateChurchRequest(
    [Required, MaxLength(200)] string Name,
    string? Address,
    string? City,
    string? Phone,
    string? Email,
    string? Description,
    string? LogoUrl,
    string? WebsiteUrl
);

public record UpdateChurchRequest(
    string? Name,
    string? Address,
    string? City,
    string? Phone,
    string? Email,
    string? Description,
    string? LogoUrl,
    string? WebsiteUrl,
    bool? IsActive
);
