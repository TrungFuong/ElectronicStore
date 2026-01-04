using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Staff
    {
        [Key]
        public string StaffId { get; set; }
        [Required]
        [MaxLength(100)]
        [Unicode]
        public string StaffName { get; set; }
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }
        [Required]
        public DateOnly StaffDOB { get; set; }
        [Required]
        public string AccountId { get; set; }
        // Navigation
        public Account? Account { get; set; }
        // Soft-delete flag
        public bool IsActive { get; set; } = true;
    }
}
