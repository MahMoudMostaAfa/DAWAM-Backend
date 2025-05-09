using Dawam_backend.Enums;
using System.ComponentModel.DataAnnotations;

namespace Dawam_backend.Models
{
    public class Job
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Job title is required.")]
        [MaxLength(100, ErrorMessage = "Job title can't exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Requirements are required.")]
        public string Requirements { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        [Required(ErrorMessage = "Career level is required.")]
        public CareerLevelE CareerLevel { get; set; }

        [Required(ErrorMessage = "Job type is required.")]
        public JobTypeE JobType { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }

        public bool IsClosed { get; set; } = false;

        [Required(ErrorMessage = "PostedBy (User ID) is required.")]
        public string PostedBy { get; set; }

        public ApplicationUser PostedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
