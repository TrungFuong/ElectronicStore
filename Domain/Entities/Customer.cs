namespace Domain.Entities
{
    public class Customer
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public DateOnly CustomerDOB { get; set; }
        public string? AccountId { get; set; }
        public Account? Account { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<DiscountUsage> DiscountUsages { get; set; } = new List<DiscountUsage>();
    }
}
