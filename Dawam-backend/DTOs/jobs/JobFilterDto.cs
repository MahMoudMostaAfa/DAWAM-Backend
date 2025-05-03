using Dawam_backend.Enums;

namespace Dawam_backend.DTOs.jobs
{
    public class JobFilterDto
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public string? Location { get; set; }
        public JobTypeE? JobType { get; set; }
        public CareerLevelE? CareerLevel { get; set; }
        public int? PageNumber { get; set; }
        public bool? SortByDateDesc { get; set; } = true;
    }

}
