using System.ComponentModel.DataAnnotations;

namespace Dawam_backend.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(100, ErrorMessage = "Category name can't exceed 100 characters.")]
        public string Name { get; set; }
    }
}
