using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class UpdateStaffRequest
    {
        [Required]
        public string StaffId { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string StaffName { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        [Required]
        public DateOnly StaffDOB { get; set; }
    }
}
