using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veltro.Models;

/// <summary>Staff member profile linked to a User account.</summary>
public class Staff
{
    [Key]
    public Guid StaffId { get; set; } = Guid.NewGuid();

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public string? Position { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
}
