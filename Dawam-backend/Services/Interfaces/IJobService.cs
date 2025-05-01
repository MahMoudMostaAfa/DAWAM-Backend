using Dawam_backend.DTOs.jobs;
using Dawam_backend.Models;

namespace Dawam_backend.Services.Interfaces
{

    public interface IJobService
    {
        Task<IEnumerable<JobDetailsDto>> GetJobsAsync(JobFilterDto filter);
        Task<JobDetailsDto?> GetJobByIdAsync(int id);
        Task<JobDetailsDto> CreateJobAsync(JobCreateDto dto, string userId);
        Task<bool> UpdateJobAsync(int id, JobUpdateDto dto, string userId);
        Task<bool> DeleteJobAsync(int id, string userId);

        Task<List<JobDetailsDto>> GetJobsByCurrentUserAsync(string userId);
    }

}
