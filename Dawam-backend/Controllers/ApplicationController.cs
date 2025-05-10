using Dawam_backend.DTOs.Applications;
using Dawam_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Dawam_backend.Models;

namespace Dawam_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IWebHostEnvironment _env;

        public ApplicationController(IApplicationService applicationService, IWebHostEnvironment env)
        {
            _applicationService = applicationService;
            _env = env;
        }

        [HttpPost("apply")]
        [Authorize(Roles = "JobApplier")]
        [RequestSizeLimit(10_000_000)] // Optional: limit upload size (10MB)
        public async Task<IActionResult> Apply([FromForm] ApplyForJobDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return a bad request if validation fails
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Save CV file to server
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "cvs");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.CVFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.CVFile.CopyToAsync(stream);
            }

            // Map DTO to Application model
            var application = new Application
            {
                JobId = dto.JobId,
                UserId = userId,
                YearsOfExperience = dto.YearsOfExperience,
                PreviousExperience = dto.PreviousExperience,
                ExpectedSalary = dto.ExpectedSalary,
                Phone = dto.Phone,
                CVFilePath = $"/cvs/{fileName}", // return relative path for frontend access
            };

            var success = await _applicationService.ApplyForJobAsync(application);
            if (!success)
                return BadRequest("You have already applied for this job.");

            return Ok("Application submitted successfully.");
        }
      
        [HttpGet("user")]
        [Authorize(Roles = "JobApplier")]
        public async Task<IActionResult> GetApplicationsForCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applications = await _applicationService.GetApplicationsByUserIdAsync(userId);
  

            return Ok(applications);
        }

        [HttpGet("job/{jobId}")]
        [Authorize(Roles = "JobPoster")]
        public async Task<IActionResult> GetApplicationsForJob(int jobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applications = await _applicationService.GetApplicationsByJobIdAsync(jobId, userId);

            if (!applications.Any())
                return NotFound("No applications found or you don't have permission for this job.");

            return Ok(applications);
        }

    }
}
