using System.ComponentModel.DataAnnotations;

namespace AutoFix_Pro.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required, Display(Name = "Full Name")]
        public string CustomerName { get; set; }

        [Required, Display(Name = "Preferred Date & Time")]
        public DateTime BookingDate { get; set; }

        [Required, Display(Name = "Service Type")]
        public string ServiceType { get; set; }

        [Required, Display(Name = "Plate Number")]
        public string VehiclePlate { get; set; }

        public string Status { get; set; } = "Pending"; // Matches your admin workflow
    }
}