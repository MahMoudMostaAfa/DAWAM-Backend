using System.ComponentModel.DataAnnotations;

namespace Dawam_backend.DTOs.Category
{
    public class CategoryDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(100, ErrorMessage = "Category name must be at most 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Category name must contain only letters and spaces.")]
        public string Name { get; set; }
    }
}
