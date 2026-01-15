namespace TimeTracking.API.Models
{
    public class TimeEntry
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal HoursWorked { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
