using System.ComponentModel.DataAnnotations;
using Dawam_backend.Enums;

namespace Dawam_backend.DTOs
{
    public class UpdateProfileDto
    {

        [RegularExpression(@"^[^\d]+$", ErrorMessage = "FullName cannot contain numbers.")]
        public string? FullName { get; set; }
        public string? Title { get; set; }
        public string? Bio { get; set; }
        public string? Address { get; set; }
        public string? Location { get; set; }
        public CareerLevelE? CareerLevel { get; set; }
        public int? ExperienceYears { get; set; }
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits.")]
        public string? Phone {  get; set; }
        public IFormFile? Image { get; set; }
       
    }
}
