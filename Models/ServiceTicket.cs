using System.ComponentModel.DataAnnotations;

namespace AutoFix_Pro.Models
{
    public class ServiceTicket
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the service name.")]
        [Display(Name = "Service Name")]
        public string ServiceName { get; set; } // e.g., Oil Change, Brake Repair

        [Required(ErrorMessage = "Please provide a description.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [Range(0, 100000, ErrorMessage = "Price must be between 0 and 100,000.")]
        [Display(Name = "Estimated Cost")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please provide an image URL for the service.")]
        [Display(Name = "Service Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL (e.g., https://example.com/image.jpg)")]
        public string ImageUrl { get; set; } // For your online image links

        [Required]
        public string Category { get; set; } // e.g., Maintenance, Diagnostics, Repair

        [Display(Name = "Date Added")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Status for the Admin Dashboard
        public bool IsActive { get; set; } = true;
    }
}