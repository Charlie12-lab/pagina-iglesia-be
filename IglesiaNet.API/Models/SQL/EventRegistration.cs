using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaNet.API.Models.SQL;

public class EventRegistration
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public string? Notes { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // FK
    [ForeignKey(nameof(Event))]
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;
}
