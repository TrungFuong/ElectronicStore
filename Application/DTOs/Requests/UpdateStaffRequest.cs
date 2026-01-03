using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class UpdateStaffRequest
    {
        public string StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? Phone { get; set; }
        public DateTime? StaffDOB { get; set; }
    }
}
