using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class UpdateBrandRequest
    {
        
        public string BrandId { get; set; }
        
        public string BrandName { get; set; }    
        public string BrandDescription { get; set; }

    }
}
