using Dawam_backend.DTOs.Analysis;

namespace Dawam_backend.Services.Interfaces
{
    public interface IAnalysisService
    {
        Task<JobAnalysisDto> GetJobAnalysisAsync(int jobId, string userId, bool isAdmin);
        Task<PlatformAnalyticsDto> GetPlatformAnalyticsAsync();
    }

}
