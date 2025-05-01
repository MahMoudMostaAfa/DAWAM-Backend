namespace Dawam_backend.DTOs.jobs
{
    public class JobCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string JobType { get; set; }
        public string CareerLevel { get; set; }
        public string Location { get; set; }
        public int? CategoryId { get; set; }
    }

}