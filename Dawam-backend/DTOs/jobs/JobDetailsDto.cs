namespace Dawam_backend.DTOs.jobs
{
    public class JobDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string JobType { get; set; }
        public string Location { get; set; }
        public string CareerLevel { get; set; }
        public bool IsClosed { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CategoryName { get; set; }
    }

}
