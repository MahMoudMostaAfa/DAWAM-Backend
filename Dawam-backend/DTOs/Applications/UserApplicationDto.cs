namespace Dawam_backend.DTOs.Applications
{
    public class UserApplicationDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        
        public string PosterName { get; set;  }
        public string JobType { get; set; }
        public string JobLevel { get; set; }
        public string JobTitle { get; set; }
        public string Status { get; set; }
       
        public DateTime AppliedAt { get; set; }
    }
}
