namespace Dawam_backend.DTOs.Applications
{
    public class ApplicationDto
    {
        public int Id { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public int YearsOfExperience { get; set; }
        public string PreviousExperience { get; set; }
        public decimal ExpectedSalary { get; set; }
        public string Phone { get; set; }
        public string CVFilePath { get; set; }
        public string Slug { get; set; }

        public DateTime AppliedAt { get; set; }
    }
}
