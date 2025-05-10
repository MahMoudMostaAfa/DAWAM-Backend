using Dawam_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dawam_backend.DTOs.Category;

namespace Dawam_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetAll()   
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return a bad request if validation fails
            }
            var category = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CategoryDto dto)
        {
            var result = await _categoryService.UpdateAsync(id, dto);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }


}
