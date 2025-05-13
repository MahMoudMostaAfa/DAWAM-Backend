using Dawam_backend.Enums;

namespace Dawam_backend.DTOs.jobs
{
    public class PageJobDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public JobTypeE JobType { get; set; }
        public string Location { get; set; }
        public CareerLevelE CareerLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CategoryName { get; set; }
    }
}
