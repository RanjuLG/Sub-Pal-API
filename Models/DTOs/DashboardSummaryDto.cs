namespace Sub_Pal_API.Models.DTOs
{
    public class DashboardSummaryDto
    {
        public decimal TotalMonthlyCost { get; set; }
        public decimal TotalAnnualCost { get; set; }
        public Dictionary<string, decimal> SpendingByCategory { get; set; } = new Dictionary<string, decimal>();
        public List<Subscription> UpcomingRenewals { get; set; } = new List<Subscription>();
    }
}
