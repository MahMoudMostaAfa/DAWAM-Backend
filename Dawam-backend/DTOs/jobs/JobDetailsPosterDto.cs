using Dawam_backend.Enums;

namespace Dawam_backend.DTOs.jobs
{
    public class JobDetailsPosterDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public JobTypeE JobType { get; set; }
        public string Location { get; set; }
        public CareerLevelE CareerLevel { get; set; }
        public bool IsClosed { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CategoryName { get; set; }
        public int ApplicationCount { get; set; }
    }
}
