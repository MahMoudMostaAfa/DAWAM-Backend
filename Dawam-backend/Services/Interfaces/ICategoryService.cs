using Dawam_backend.DTOs.Category;
using Dawam_backend.Models;
namespace Dawam_backend.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<GetCategoryDto>> GetAllAsync();
        Task<Category> CreateAsync(CategoryDto dto);
        Task<bool> UpdateAsync(int id, CategoryDto dto);
        Task<bool> DeleteAsync(int id);
    }


}
