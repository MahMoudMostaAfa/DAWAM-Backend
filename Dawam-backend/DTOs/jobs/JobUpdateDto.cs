using Dawam_backend.Enums;
using System.ComponentModel.DataAnnotations;

namespace Dawam_backend.DTOs.jobs
{
    public class JobUpdateDto
    {
        [MaxLength(100)]
        public string? Title { get; set; }

        [MinLength(10)]
        public string? Description { get; set; }

        [MinLength(10)]
        public string? Requirements { get; set; }

        public CareerLevelE? CareerLevel { get; set; }

        public JobTypeE? JobType { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }

        public bool? IsClosed { get; set; }

        public int? CategoryId { get; set; }
    }
}
