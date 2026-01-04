using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class CreateProductRequest
    {
        public string ProductName { get; set; } = null!;
        public string ProductDescription { get; set; } = null!;
        public string CategoryId { get; set; } = null!;
        public string BrandId { get; set; } = null!;
        public bool? IsActive { get; set; }

        public List<ProductVariationRequest> Variations { get; set; } = new();

        public List<ProductSpecificationRequest> Specifications { get; set; } = new();

        public List<ProductImageRequest> Images { get; set; } = new();
    }

    public class ProductVariationRequest
    {
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public List<VariationOptionRequest> Options { get; set; } = new();
    }

    public class VariationOptionRequest
    {
        public int AttributeId { get; set; }
        public string OptionValue { get; set; } = null!;
    }

    public class ProductSpecificationRequest
    {
        public string SpecKey { get; set; } = null!;
        public string SpecValue { get; set; } = null!;
    }

    public class ProductImageRequest
    {
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
    }
}
