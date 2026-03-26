namespace AutoFix_Pro.Models
{
    public class AdminDashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalAppointments { get; set; }
        public int PendingTasks { get; set; }

        // Data for the Charts
        public decimal ProjectedRevenue { get; set; }
        public List<string> ChartLabels { get; set; } = new();
        public List<int> ChartData { get; set; } = new();
    }
}a