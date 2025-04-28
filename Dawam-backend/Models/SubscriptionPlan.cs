namespace Dawam_backend.Models
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MonthlyApplicationLimit { get; set; }
        public decimal Price { get; set; }
        public string StripePlanId { get; set; }
    }
}
