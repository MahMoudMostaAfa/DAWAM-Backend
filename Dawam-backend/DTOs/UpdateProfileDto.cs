using Dawam_backend.Enums;

namespace Dawam_backend.DTOs
{
    public class UpdateProfileDto
    {
       
        public string? FullName { get; set; }
        public string? Title { get; set; }
        public string? Bio { get; set; }
        public string? Address { get; set; }
        public string? Location { get; set; }
        public CareerLevelE? CareerLevel { get; set; }
        public int? ExperienceYears { get; set; }
        public IFormFile? Image { get; set; }
       
    }
}
