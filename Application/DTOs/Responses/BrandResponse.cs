using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class BrandResponse
    {
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public string? BrandDescription { get; set; }
    }
}
