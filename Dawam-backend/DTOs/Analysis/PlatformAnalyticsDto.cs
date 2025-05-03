namespace Dawam_backend.DTOs.Analysis
{
    public class PlatformAnalyticsDto
    {
        public int TotalUsers { get; set; }
        public int TotalJobs { get; set; }
        public int TotalApplications { get; set; }

        public int JobAppliersCount { get; set; }
        public int JobPostersCount { get; set; }

        public int FullTimeJobs { get; set; }
        public int PartTimeJobs { get; set; }
        public int RemoteJobs { get; set; }

        public int SeniorJobs { get; set; }
        public int JuniorJobs { get; set; }
        public int FreshJobs { get; set; }
        public int InternshipJobs { get; set; }

        public Dictionary<string, int> MonthlyApplications { get; set; } // e.g., "Jan 2025" => 35
    }
}
