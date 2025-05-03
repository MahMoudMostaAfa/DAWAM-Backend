using System.Globalization;
using Dawam_backend.Data;
using Dawam_backend.DTOs.Analysis;
using Dawam_backend.Enums;
using Dawam_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dawam_backend.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly ApplicationDbContext _context;

        public AnalysisService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<JobAnalysisDto> GetJobAnalysisAsync(int jobId, string userId, bool isAdmin)
        {
            // Check if user has access

            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
            bool hasAccess = isAdmin || await _context.Payments.AnyAsync(p => p.UserId == userId && p.PaymentDate >= oneMonthAgo);
            if (!hasAccess)
                throw new UnauthorizedAccessException("Access denied.");

            // Get applications
            var applications = await _context.Applications
                .Where(a => a.JobId == jobId)
                .Include(a => a.User)
                .ToListAsync();

            if (!applications.Any())
            {
                return new JobAnalysisDto
                {
                    TotalApplications = 0,
                    AvgExpectedSalary = 0,
                    AvgYearsOfExperience = 0,
                    CareerLevelCounts = new Dictionary<string, int>
                    {
                        { "Senior", 0 },
                        { "Junior", 0 },
                        { "Fresh", 0 },
                        { "Internship", 0 }
                    }
                };
            }
            var careerLevelCounts = applications
                .GroupBy(a => Enum.GetName(typeof(CareerLevelE), a.User.CareerLevel) ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var level in new[] { "Senior", "Junior", "Fresh", "Internship" })
            {
                if (!careerLevelCounts.ContainsKey(level))
                    careerLevelCounts[level] = 0;
            }

            return new JobAnalysisDto
            {
                TotalApplications = applications.Count,
                AvgExpectedSalary = applications.Average(a => a.ExpectedSalary),
                AvgYearsOfExperience = applications.Average(a => a.YearsOfExperience),
                CareerLevelCounts = careerLevelCounts
            };
        }

        public async Task<PlatformAnalyticsDto> GetPlatformAnalyticsAsync()
        {
            var oneYearAgo = DateTime.UtcNow.AddMonths(-11);
            var jobs = _context.Jobs.AsQueryable();
            var activeJobs = _context.Jobs.Where(j => j.IsClosed == false);
            var activeUsers = _context.Users.Where(u => u.IsActive);
            var jobApplierRoleId = await _context.Roles
                .Where(r => r.Name == "JobApplier")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();
            var jobPosterRoleId = await _context.Roles
                .Where(r => r.Name == "JobPoster")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();
            var jobAppliersCount = await _context.UserRoles
                .Where(ur => ur.RoleId == jobApplierRoleId)
                .Join(_context.Users.Where(u => u.IsActive),
                      ur => ur.UserId,
                      u => u.Id,
                      (ur, u) => u)
                .CountAsync();
            var jobPostersCount = await _context.UserRoles
                .Where(ur => ur.RoleId == jobPosterRoleId)
                .Join(_context.Users.Where(u => u.IsActive),
                      ur => ur.UserId,
                      u => u.Id,
                      (ur, u) => u)
                .CountAsync();


            var monthApplications = Enumerable.Range(0, 12)
                .Select(i => DateTime.UtcNow.AddMonths(-i))
                .Select(d => new DateTime(d.Year, d.Month, 1))
                .OrderBy(d => d)
                .ToList();

            // This should go into a different variable, not overwrite monthApplications
            var monthlyCounts = await _context.Applications
                .Where(a => a.AppliedAt >= oneYearAgo)
                .GroupBy(a => new { a.AppliedAt.Year, a.AppliedAt.Month })
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Count = g.Count()
                })
                .ToListAsync();

            return new PlatformAnalyticsDto
            {
                TotalUsers = await activeUsers.CountAsync(),
                TotalJobs = await activeJobs.CountAsync(),
                TotalApplications = await _context.Applications.CountAsync(),

                JobAppliersCount = jobAppliersCount,
                JobPostersCount = jobPostersCount,

                FullTimeJobs = await activeJobs.CountAsync(j => j.JobType == JobTypeE.FullTime),
                PartTimeJobs = await activeJobs.CountAsync(j => j.JobType == JobTypeE.PartTime),
                RemoteJobs = await activeJobs.CountAsync(j => j.JobType == JobTypeE.Remote),

                SeniorJobs = await activeJobs.CountAsync(j => j.CareerLevel == CareerLevelE.Senior),
                JuniorJobs = await activeJobs.CountAsync(j => j.CareerLevel == CareerLevelE.Junior),
                FreshJobs = await activeJobs.CountAsync(j => j.CareerLevel == CareerLevelE.Fresh),
                InternshipJobs = await activeJobs.CountAsync(j => j.CareerLevel == CareerLevelE.Internship),

                MonthlyApplications = monthApplications
                    .ToDictionary(
                        m => m.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                        m => monthlyCounts.FirstOrDefault(x => x.Month == m)?.Count ?? 0
                        )
            };
        }
    }

}
