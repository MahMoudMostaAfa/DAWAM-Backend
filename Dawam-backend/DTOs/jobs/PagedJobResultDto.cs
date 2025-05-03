namespace Dawam_backend.DTOs.jobs
{
    public class PagedJobResultDto
    {
        public int TotalCount { get; set; }
        public List<JobDetailsDto> Jobs { get; set; }
    }
}
