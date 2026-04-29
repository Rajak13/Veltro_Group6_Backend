using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veltro.Models;

/// <summary>Customer review and rating for the service.</summary>
public class Review
{
    [Key]
    public Guid ReviewId { get; set; } = Guid.NewGuid();

    [ForeignKey(nameof(Customer))]
    public Guid CustomerId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Comment { get; set; }

    public bool IsApproved { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Customer Customer { get; set; } = null!;
}
