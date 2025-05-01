namespace Dawam_backend.DTOs.Applications
{
    public class ApplyForJobDto
    {
        public int JobId { get; set; }
        public int YearsOfExperience { get; set; }
        public string PreviousExperience { get; set; }
        public decimal ExpectedSalary { get; set; }
        public string Phone { get; set; }
        public IFormFile CVFile { get; set; } // Uploaded file
    }
}
