using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class ProductSpecificationResponse
    {
        public string SpecificationId { get; set; }
        public string SpecKey { get; set; } = null!;
        public string SpecValue { get; set; } = null!;
    }
}
