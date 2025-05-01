using Dawam_backend.Data;
using Dawam_backend.DTOs;
using Dawam_backend.DTOs.jobs;
using Dawam_backend.Models;
using Dawam_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Dawam_backend.Services
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _context;

        public JobService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobDetailsDto>> GetJobsAsync(JobFilterDto filter)
        {
            var query = _context.Jobs
                .Include(j => j.Category)
                .Where(j => !j.IsClosed)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Location))
                query = query.Where(j => j.Location.Contains(filter.Location));

            if (!string.IsNullOrEmpty(filter.Category))
                query = query.Where(j => j.Category.Name.Contains(filter.Category));

            if (!string.IsNullOrEmpty(filter.JobType))
                query = query.Where(j => j.JobType == filter.JobType);

            return await query.Select(j => new JobDetailsDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                Requirements = j.Requirements,
                JobType = j.JobType,
                Location = j.Location,
                CareerLevel = j.CareerLevel,
                CreatedAt = j.CreatedAt,
                IsClosed = j.IsClosed,
                CategoryName = j.Category.Name
            }).ToListAsync();
        }

        public async Task<JobDetailsDto?> GetJobByIdAsync(int id)
        {
            var job = await _context.Jobs.Include(j => j.Category)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return null;

            return new JobDetailsDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                JobType = job.JobType,
                Location = job.Location,
                CareerLevel = job.CareerLevel,
                CreatedAt = job.CreatedAt,
                IsClosed = job.IsClosed,
                CategoryName = job.Category?.Name
            };
        }

        public async Task<JobDetailsDto> CreateJobAsync(JobCreateDto dto, string userId)
        {
            var job = new Job
            {
                Title = dto.Title,
                Description = dto.Description,
                Requirements = dto.Requirements,
                JobType = dto.JobType,
                CareerLevel = dto.CareerLevel,
                Location = dto.Location,
                CategoryId = dto.CategoryId,
                PostedBy = userId
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return await GetJobByIdAsync(job.Id); // Reuse method to return full DTO
        }

        public async Task<bool> UpdateJobAsync(int id, JobUpdateDto dto, string userId)
        {
            var job = await _context.Jobs.FindAsync(id);

            if (job == null || job.PostedBy != userId) return false;

            job.Title = dto.Title;
            job.Description = dto.Description;
            job.Requirements = dto.Requirements;
            job.JobType = dto.JobType;
            job.CareerLevel = dto.CareerLevel;
            job.Location = dto.Location;
            job.CategoryId = dto.CategoryId;
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteJobAsync(int id, string userId)
        {
            var job = await _context.Jobs.FindAsync(id);

            if (job == null || job.PostedBy != userId) return false;

            job.IsClosed = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<JobDetailsDto>> GetJobsByCurrentUserAsync(string userId)
        {
            return await _context.Jobs
                .Include(j => j.Category)
                .Where(j => j.PostedBy == userId && !j.IsClosed)
                .Select(j => new JobDetailsDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Requirements = j.Requirements,
                    JobType = j.JobType,
                    Location = j.Location,
                    CareerLevel = j.CareerLevel,
                    CreatedAt = j.CreatedAt,
                    IsClosed = j.IsClosed,
                    CategoryName = j.Category.Name
                })
                .ToListAsync();
        }
    }
}
