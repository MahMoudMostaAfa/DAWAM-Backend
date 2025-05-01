using Dawam_backend.DTOs.Applications;
using Dawam_backend.Models;
namespace Dawam_backend.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<bool> ApplyForJobAsync(Application application);
        Task<IEnumerable<UserApplicationDto>> GetApplicationsByUserIdAsync(string userId);
        Task<IEnumerable<ApplicationDto>> GetApplicationsByJobIdAsync(int jobId, string jobPosterId);

    }
}
