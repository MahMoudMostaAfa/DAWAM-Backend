using Dawam_backend.DTOs;
using Dawam_backend.DTOs.jobs;
using Dawam_backend.Models;
using Dawam_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dawam_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs([FromQuery] JobFilterDto filter)
        {
            var jobs = await _jobService.GetJobsAsync(filter);
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null) return NotFound();

            return Ok(job);
        }

        [Authorize(Roles = "JobPoster")]
        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] JobCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var job = await _jobService.CreateJobAsync(dto, userId);
            return CreatedAtAction(nameof(GetJobById), new { id = job.Id }, job);
        }

        [Authorize(Roles = "JobPoster")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] JobUpdateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _jobService.UpdateJobAsync(id, dto, userId);
            if (!success) return Forbid();

            return NoContent();
        }

        [Authorize(Roles = "JobPoster")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _jobService.DeleteJobAsync(id, userId);
            if (!success) return Forbid();

            return NoContent();
        }

        [Authorize(Roles = "JobPoster")]
        [HttpGet("my-jobs")]
        public async Task<IActionResult> GetMyJobs()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = await _jobService.GetJobsByCurrentUserAsync(userId);
            return Ok(jobs);
        }
    }
}
