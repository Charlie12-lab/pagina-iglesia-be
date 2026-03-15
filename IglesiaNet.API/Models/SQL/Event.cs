using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaNet.API.Models.SQL;

public class Event
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [MaxLength(300)]
    public string? Location { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public bool AllowsRegistration { get; set; } = false;

    public int? MaxAttendees { get; set; }

    public bool IsPublished { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK
    [ForeignKey(nameof(Church))]
    public int ChurchId { get; set; }
    public Church Church { get; set; } = null!;

    // Navigation
    public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
}
