using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class CreateBrandRequest
    {
        [Required]
        public string BrandName { get; set; }
        public string? BrandDescription { get; set; }
    

    }
}
