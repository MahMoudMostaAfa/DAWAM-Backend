using Dawam_backend.Data;
using Dawam_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Dawam_backend.Models;
using Dawam_backend.DTOs.Applications;
using Dawam_backend.DTOs.jobs;

namespace Dawam_backend.Services
{
    public class ApplicationService:IApplicationService
    {
        private readonly ApplicationDbContext _context;

        public ApplicationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ApplyForJobAsync(Application application)
        {
            // Check if user already applied for this job
            bool alreadyApplied = await _context.Applications.AnyAsync(a => a.UserId == application.UserId && a.JobId == application.JobId);
            if (alreadyApplied) return false;

            // Save application
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<UserApplicationDto>> GetApplicationsByUserIdAsync(string userId)
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

            var applications = await _context.Applications
                .Where(a => a.UserId == userId)
                .Select(a => new
                {
                    Application = a,
                    HasValidPayment = _context.Payments.Any(p =>
                        p.UserId == a.UserId &&
                        p.PaymentDate >= oneMonthAgo)
                })
                .OrderByDescending(a => a.HasValidPayment) // Payments first
                .ThenByDescending(a => a.Application.AppliedAt)
                .Select(a => new UserApplicationDto
                {
                    Id = a.Application.Id,
                    JobId = a.Application.JobId,
                    JobTitle = a.Application.Job.Title,
                    AppliedAt = a.Application.AppliedAt,
                    Status = a.Application.Status,
                    PosterName = a.Application.Job.PostedByUser.FullName ,
                    CareerLevel = a.Application.Job.CareerLevel,
                    JobType = a.Application.Job.JobType
                })
                .ToListAsync();
            var applicationsWithLocal = applications.Select(a => new UserApplicationDto
            {
                Id = a.Id,
                JobId = a.JobId,
                JobTitle = a.JobTitle,
                AppliedAt = TimeZoneInfo.ConvertTimeFromUtc(a.AppliedAt, TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")),
                Status = a.Status,
                PosterName = a.PosterName,
                CareerLevel = a.CareerLevel,
                JobType = a.JobType
            }).ToList();

            return applicationsWithLocal;
        }

        public async Task<IEnumerable<ApplicationDto>> GetApplicationsByJobIdAsync(int jobId, string jobPosterId)
        {
          var applications= await _context.Applications
         .Where(a => a.JobId == jobId && a.Job.PostedBy == jobPosterId)
         .Select(a => new ApplicationDto
         {
             Id = a.Id,
             UserFullName = a.User.FullName,
             UserEmail = a.User.Email,
             AppliedAt = a.AppliedAt,
             Phone=a.Phone,
             PreviousExperience=a.PreviousExperience,
             YearsOfExperience=a.YearsOfExperience,
             ExpectedSalary=a.ExpectedSalary,
             CVFilePath=a.CVFilePath,
             Slug=a.User.Slug,
             ImagePath=a.User.ImagePath
            
           
         })
         .ToListAsync();

             var applicationsWithLocal = applications.Select(a => new ApplicationDto
             {
                 Id = a.Id,
                 UserFullName = a.UserFullName,
                 UserEmail = a.UserEmail,
                 AppliedAt = TimeZoneInfo.ConvertTimeFromUtc(a.AppliedAt, TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")),
                 Phone = a.Phone,
                 PreviousExperience = a.PreviousExperience,
                 YearsOfExperience = a.YearsOfExperience,
                 ExpectedSalary = a.ExpectedSalary,
                 CVFilePath = a.CVFilePath,
                 Slug = a.Slug,
                 ImagePath = a.ImagePath
             }).ToList();

            return applicationsWithLocal;
        }



    }
}
