using Dawam_backend.Enums;

namespace Dawam_backend.DTOs.jobs
{
    public class JobUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Requirements { get; set; }
        public CareerLevelE? CareerLevel { get; set; }
        public JobTypeE? JobType { get; set; }
        public string? Location { get; set; }
        public bool? IsClosed { get; set; }
        public int? CategoryId { get; set; }
    }

}
