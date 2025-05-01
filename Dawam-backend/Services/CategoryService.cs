using Dawam_backend.Data;
using Dawam_backend.DTOs.Category;
using Dawam_backend.Models;
using Dawam_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dawam_backend.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetCategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .Select(c => new GetCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<Category> CreateAsync(CategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<bool> UpdateAsync(int id, CategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            category.Name = dto.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }



    }


}
