using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veltro.Models;

/// <summary>Customer request for a part not currently in stock.</summary>
public class PartRequest
{
    [Key]
    public Guid RequestId { get; set; } = Guid.NewGuid();

    [ForeignKey(nameof(Customer))]
    public Guid CustomerId { get; set; }

    [Required]
    public string PartName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public PartRequestStatus Status { get; set; } = PartRequestStatus.Pending;

    // Navigation
    public Customer Customer { get; set; } = null!;
}
