using Dawam_backend.DTOs.jobs;
using Dawam_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dawam_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "JobApplier")]
    public class SavedJobsController : ControllerBase
    {
        private readonly ISavedJobService _savedJobService;

        public SavedJobsController(ISavedJobService savedJobService)
        {
            _savedJobService = savedJobService;
        }

        [HttpGet]

        public async Task<IActionResult> GetSavedJobs()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var savedJobs = await _savedJobService.GetSavedJobsAsync(userId);
            return Ok(savedJobs);
        }

        [HttpPost("{jobId}")]
        public async Task<IActionResult> SaveJob(int jobId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var saved = await _savedJobService.SaveJobAsync(userId, jobId);
                return saved ? Ok("Job saved successfully.") : BadRequest("Job already saved.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{jobId}")]
        public async Task<IActionResult> UnsaveJob(int jobId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var removed = await _savedJobService.UnsaveJobAsync(userId, jobId);
            return removed ? Ok("Saved job removed.") : NotFound("Saved job not found.");
        }
    }

}
