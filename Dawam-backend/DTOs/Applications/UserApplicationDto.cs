using Dawam_backend.Enums;

namespace Dawam_backend.DTOs.Applications
{
    public class UserApplicationDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        
        public string PosterName { get; set;  }
        public JobTypeE JobType { get; set; }
        public CareerLevelE CareerLevel { get; set; }
        public string JobTitle { get; set; }
        public string Status { get; set; }
       
        public DateTime AppliedAt { get; set; }
    }
}
