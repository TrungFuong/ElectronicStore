using System;

namespace Application.DTOs.Responses
{
    public class StaffResponse
    {
        public string StaffId { get; set; } = null!;
        public string StaffName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateOnly StaffDOB { get; set; }
        public string AccountId { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
