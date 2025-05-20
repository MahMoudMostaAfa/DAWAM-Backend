using Dawam_backend.Data;
using Dawam_backend.DTOs;
using Dawam_backend.DTOs.jobs;
using Dawam_backend.Models;
using Dawam_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Stripe;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Dawam_backend.Services
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _context;

        public JobService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedJobResultDto> GetJobsAsync(JobFilterDto filter)
        {
            var query = _context.Jobs
                .Include(j => j.Category)
                .Where(j => !j.IsClosed)
                .AsQueryable();

            // Filter by title (search)
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                string normalizedSearch = filter.SearchTerm.Replace("-", " ");
                query = query.Where(j => j.Title.Contains(normalizedSearch));
            }

            // Filter by category
            if (!string.IsNullOrEmpty(filter.Category))
                query = query.Where(j => j.Category.Name.Contains(filter.Category));

            // Filter by location
            if (!string.IsNullOrEmpty(filter.Location))
                query = query.Where(j => j.Location.Contains(filter.Location));

            // Filter by job type
            if (filter.JobType.HasValue)
                query = query.Where(j => j.JobType == filter.JobType.Value);

            // Filter by career level
            if (filter.CareerLevel.HasValue)
                query = query.Where(j => j.CareerLevel == filter.CareerLevel.Value);


            int totalCount = await query.CountAsync();

            // Sort by CreatedAt
            if (filter.SortByDateDesc.HasValue && filter.SortByDateDesc.Value)
                query = query.OrderByDescending(j => j.CreatedAt);
            else
                query = query.OrderBy(j => j.CreatedAt);

            // Pagination
            int page = filter.PageNumber.HasValue && filter.PageNumber.Value > 0
                ? filter.PageNumber.Value
                : 1;
            if (page < 1) page = 1;  // Set to 1 if the page number is less than 1
            int pageSize = 10;
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var jobs = await query.Select(j => new PageJobDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                JobType = j.JobType,
                Location = j.Location,
                CareerLevel = j.CareerLevel,
                CreatedAt = j.CreatedAt,
                CategoryName = j.Category.Name
            }).ToListAsync();

            var jobsWithLocalTime = jobs.Select(j => new PageJobDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                JobType = j.JobType,
                Location = j.Location,
                CareerLevel = j.CareerLevel,
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(j.CreatedAt, TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")),
                CategoryName = j.CategoryName
            }).ToList();

            return new PagedJobResultDto
            {
                TotalCount = totalCount,
                Jobs = jobsWithLocalTime
            };
        }


        public async Task<JobDetailsDto?> GetJobByIdAsync(int id, string userId)
        {
            var job = await _context.Jobs.Include(j => j.Category)
                .FirstOrDefaultAsync(j => j.Id == id);

            var IsApplied = await _context.Applications.AnyAsync(j => j.JobId == id && j.UserId==userId);
            bool IsSaved = await _context.SavedJobs.AnyAsync(s => s.UserId == userId && s.JobId == id);

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
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(job.CreatedAt, TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")),
                IsClosed = job.IsClosed,
                IsApplied = IsApplied,
                IsSaved = IsSaved,
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

            return await GetJobByIdAsync(job.Id,null); // Reuse method to return full DTO
        }

        public async Task<bool> UpdateJobAsync(int id, JobUpdateDto dto, string userId)
        {
            var job = await _context.Jobs.FindAsync(id);

            if (job == null || job.PostedBy != userId) return false;

            if (!string.IsNullOrEmpty(dto.Title)) job.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.Description)) job.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.Requirements)) job.Requirements = dto.Requirements;
            if(dto.JobType.HasValue) job.JobType = dto.JobType.Value;
            if (dto.CareerLevel.HasValue) job.CareerLevel = dto.CareerLevel.Value;
            if (!string.IsNullOrEmpty(dto.Location)) job.Location = dto.Location;
            if( dto.CategoryId.HasValue) job.CategoryId = dto.CategoryId.Value;
            if (dto.IsClosed.HasValue)
            {
                job.IsClosed = dto.IsClosed.Value; // Explicitly get the value
            }

            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteJobAsync(int id, string userId, string userRole)
        {
            
            var job = await _context.Jobs.FindAsync(id);
           
            if (job == null) return false;

            
            if (userRole == "Admin")
            {
                _context.Jobs.Remove(job);
            }else
            {
                if (job.PostedBy != userId) return false;
                job.IsClosed = true;
            }
           
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<JobDetailsPosterDto>> GetJobsByCurrentUserAsync(string userId)
        {
            return await _context.Jobs
                .Include(j => j.Category)
                .Where(j => j.PostedBy == userId )
                .Select(j => new JobDetailsPosterDto
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
                    CategoryName = j.Category.Name,
                    ApplicationCount = _context.Applications.Count(a => a.JobId == j.Id)
                })
                .ToListAsync();
        }
    }
}
