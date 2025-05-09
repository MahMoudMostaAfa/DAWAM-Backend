using System.ComponentModel.DataAnnotations;

namespace Dawam_backend.Models
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name can't exceed 100 characters.")]
        public string Name { get; set; }

        //[Range(0, 1000, ErrorMessage = "Limit must be between 0 and 1000.")]
        //public int MonthlyApplicationLimit { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, 100000, ErrorMessage = "Price must be between 1 and 100,000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stripe Plan ID is required.")]
        [MaxLength(100, ErrorMessage = "Stripe Plan ID can't exceed 100 characters.")]
        public string StripePlanId { get; set; }
    }
}
