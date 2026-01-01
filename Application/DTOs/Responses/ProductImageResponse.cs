using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class ProductImageResponse
    {
        public string ImageId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
    }
}
