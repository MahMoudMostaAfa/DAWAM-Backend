using System.ComponentModel.DataAnnotations;

namespace Dawam_backend.Models
{
    public class SavedJob
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "JobId is required.")]
        public int JobId { get; set; }

        public ApplicationUser User { get; set; }

        public Job Job { get; set; }

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
