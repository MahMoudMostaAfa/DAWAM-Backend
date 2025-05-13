using Dawam_backend.Data;
using Dawam_backend.DTOs.jobs;
using Dawam_backend.Enums;
using Dawam_backend.Models;
using Dawam_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dawam_backend.Services
{
    public class SavedJobService : ISavedJobService
    {
        private readonly ApplicationDbContext _context;

        public SavedJobService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PageJobDto>> GetSavedJobsAsync(string userId)
        {
            var savedJobs = await _context.SavedJobs
                .Where(s => s.UserId == userId)
                .Include(j => j.Job.Category)
                .Select(s => new PageJobDto
                    {
                        Id = s.Job.Id,
                        Title = s.Job.Title,
                        Description = s.Job.Description,
                        JobType = s.Job.JobType,
                        Location = s.Job.Location,
                        CareerLevel = s.Job.CareerLevel,
                        CreatedAt = s.Job.CreatedAt,
                        CategoryName = s.Job.Category.Name,
                    })
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return savedJobs;
        }

        public async Task<bool> SaveJobAsync(string userId, int jobId)
        {
            bool alreadySaved = await _context.SavedJobs
                .AnyAsync(s => s.UserId == userId && s.JobId == jobId);

            if (alreadySaved) return false;

            bool jobExists = await _context.Jobs.AnyAsync(j => j.Id == jobId);
            if (!jobExists) throw new ArgumentException("Job not found.");

            var savedJob = new SavedJob
            {
                UserId = userId,
                JobId = jobId,
                SavedAt = DateTime.UtcNow
            };

            _context.SavedJobs.Add(savedJob);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnsaveJobAsync(string userId, int jobId)
        {
            var savedJob = await _context.SavedJobs
                .FirstOrDefaultAsync(s => s.UserId == userId && s.JobId == jobId);

            if (savedJob == null) return false;

            _context.SavedJobs.Remove(savedJob);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
