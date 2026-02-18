using System.ComponentModel.DataAnnotations;

namespace AutoFix_Pro.Models;

public class ServiceTicket
{
    public int Id { get; set; }

    [Display(Name = "Client Name")]
    public string? ClientName { get; set; }

    [Display(Name = "Vehicle Plate")]
    public string? PlateNumber { get; set; }

    [Display(Name = "Service Type")]
    public string? ServiceType { get; set; } // e.g., Engine Oil Change

    [DataType(DataType.Date)]
    [Display(Name = "Job Date")]
    public DateTime JobDate { get; set; }

    public decimal Price { get; set; }
}