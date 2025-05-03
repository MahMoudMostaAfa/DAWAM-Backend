using System.ComponentModel.DataAnnotations;

namespace Dawam_backend.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public string CareerLevel { get; set; }
        public string JobType { get; set; }
        public string Location { get; set; }
        public bool IsClosed { get; set; } = false;
        public string PostedBy { get; set; }
        public ApplicationUser PostedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Application> Applications { get; set; } = new List<Application>();

    }
}
