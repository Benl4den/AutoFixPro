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
        // Regex breakdown: 
        // ^[A-Z]{3}  -> Exactly 3 uppercase letters at the start
        // [ ]        -> Exactly one space
        // \d+$       -> One or more digits until the end
        [RegularExpression(@"^[A-Z]{3} \d+$", ErrorMessage = "Format must be 3 Letters followed by a space and numbers (e.g., ABC 123)")]
        public string VehiclePlate { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Pending";
    }
}