using Dawam_backend.Data;
using Dawam_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Dawam_backend.Models;
using Dawam_backend.DTOs.Applications;

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
            return await _context.Applications
                //.Include(a => a.Job)
                .Where(a => a.UserId == userId)
                .Select(a => new UserApplicationDto
                {
                    Id=a.Id,
                    JobId=a.JobId,
                    JobTitle=a.Job.Title,
                    AppliedAt=a.AppliedAt,
                    Status=a.Status,
                    PosterName=a.User.UserName,
                    CareerLevel=a.Job.CareerLevel,
                    JobType=a.Job.JobType,


                   
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationDto>> GetApplicationsByJobIdAsync(int jobId, string jobPosterId)
        {
            return await _context.Applications
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
             CVFilePath=a.CVFilePath
           
         })
         .ToListAsync();
        }



    }
}
