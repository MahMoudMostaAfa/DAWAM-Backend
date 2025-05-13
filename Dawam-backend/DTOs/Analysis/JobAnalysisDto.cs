namespace Dawam_backend.DTOs.Analysis
{
    public class JobAnalysisDto
    {
        public int TotalApplications { get; set; }
        public decimal AvgExpectedSalary { get; set; }
        public double AvgYearsOfExperience { get; set; }

        public Dictionary<string, int> CareerLevelCounts { get; set; } = new();
    }
}
