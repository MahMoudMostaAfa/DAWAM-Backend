using Dawam_backend.Enums;
using System.ComponentModel.DataAnnotations;

namespace Dawam_backend.DTOs.jobs
{
    public class JobCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MinLength(10)]
        public string Description { get; set; }

        [Required]
        [MinLength(10)]
        public string Requirements { get; set; }

        [Required]
        public JobTypeE JobType { get; set; }

        [Required]
        public CareerLevelE CareerLevel { get; set; }

        [Required]
        [MaxLength(100)]
        public string Location { get; set; }

        [Required]
        public int? CategoryId { get; set; }
    }
}
