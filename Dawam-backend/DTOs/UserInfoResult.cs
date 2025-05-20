namespace Dawam_backend.DTOs
{
    public class UserInfoResult
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Bio { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public int? CareerLevel { get; set; }
        public int? ExperienceYears { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public bool IsPremium { get; set; }
        public string ImagePath { get; set; }
    }
}
