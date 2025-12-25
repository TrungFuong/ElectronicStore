using Domain.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class AddStaffRequest
    {
        public string Phone { get; set; }
        public string StaffName { get; set; }
        public DateOnly StaffDOB { get; set; }
    }
}
