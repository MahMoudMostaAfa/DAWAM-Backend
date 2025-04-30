namespace Dawam_backend.DTOs
{
    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? Title { get; set; }
        public string? Bio { get; set; }
        public string? Address { get; set; }
        public string? Location { get; set; }
        public string? CareerLevel { get; set; }
        public int? ExperienceYears { get; set; }
    }
}
