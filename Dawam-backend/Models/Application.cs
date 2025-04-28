namespace Dawam_backend.Models
{
    public class Application
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int YearsOfExperience { get; set; }
        public string PreviousExperience { get; set; }
        public decimal ExpectedSalary { get; set; }
        public string Phone { get; set; }
        public string CVFilePath { get; set; }
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";

    }
}
