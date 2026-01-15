using System.ComponentModel.DataAnnotations;

namespace TimeTracking.API.Dtos
{
    public class TimeEntryDto
    {
        [Required, MaxLength(30)]
        public string EmployeeName { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Range(0.1, 24.0, ErrorMessage = "Hours worked must be between 0.1 and 24.")]
        public decimal HoursWorked { get; set; }
        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;

    }
}
