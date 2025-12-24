using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class UpdateProductSpecificationRequest
    {
        public string SpecificationId { get; set; } = string.Empty;
        public string SpecKey { get; set; } = string.Empty;
        public string SpecValue { get; set; } = string.Empty;
    }
}
