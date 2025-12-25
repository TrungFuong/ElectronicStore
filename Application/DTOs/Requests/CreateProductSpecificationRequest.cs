using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class CreateProductSpecificationRequest
    {
        public string? ProductId { get; set; }
        public List<ProductSpecificationItem> Specifications { get; set; } = new();
    }

    public class ProductSpecificationItem
    {
        public string SpecKey { get; set; } = null!;
        public string SpecValue { get; set; } = null!;
    }
}
