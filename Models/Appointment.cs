using System.ComponentModel.DataAnnotations;

namespace AutoFix_Pro.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Preferred Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime BookingDate { get; set; }

        [Required]
        [Display(Name = "Service Type")]
        public string ServiceType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Plate Number")]
        [RegularExpression(@"^[A-Z0-9\s-]+$", ErrorMessage = "Invalid Plate Number format")]
        public string VehiclePlate { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Pending";
    }
}