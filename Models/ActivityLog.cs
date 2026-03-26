namespace AutoFix_Pro.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } // Who did it
        public string Action { get; set; }    // e.g., "Updated Status", "Deleted Service"
        public string Details { get; set; }   // e.g., "Changed Appointment #10 to Completed"
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
