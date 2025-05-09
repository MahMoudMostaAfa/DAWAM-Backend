using Dawam_backend.DTOs.jobs;

namespace Dawam_backend.Services.Interfaces
{
    public interface ISavedJobService
    {
        Task<List<PageJobDto>> GetSavedJobsAsync(string userId);
        Task<bool> SaveJobAsync(string userId, int jobId);
        Task<bool> UnsaveJobAsync(string userId, int jobId);
    }

}
