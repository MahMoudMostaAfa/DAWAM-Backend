
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Dawam_backend.Services.Interfaces;
using Dawam_backend.DTOs.Analysis;

namespace Dawam_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly IAnalysisService _analysisService;

        public AnalysisController(IAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,JobApplier")]
        public async Task<IActionResult> GetJobAnalysis(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            try
            {
                var result = await _analysisService.GetJobAnalysisAsync(id, userId, isAdmin);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PlatformAnalyticsDto>> GetPlatformAnalytics()
        {
            var result = await _analysisService.GetPlatformAnalyticsAsync();
            return Ok(result);
        }
    }
}


