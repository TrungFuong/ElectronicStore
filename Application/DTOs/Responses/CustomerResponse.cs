using System;

namespace Application.DTOs.Responses
{
    public class CustomerResponse
    {
        public string CustomerId { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string CustomerAddress { get; set; } = null!;
        public DateOnly CustomerDOB { get; set; }
        public string? AccountId { get; set; }
    }
}
