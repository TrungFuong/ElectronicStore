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
        // ===== PRODUCT (KHÔNG CÓ GIÁ & TỒN) =====
        public string ProductName { get; set; } = null!;
        public string ProductDescription { get; set; } = null!;
        public string CategoryId { get; set; } = null!;
        public string BrandId { get; set; } = null!;

        // ===== VARIATIONS (GIÁ + TỒN Ở ĐÂY) =====
        public List<ProductVariationRequest> Variations { get; set; } = new();

        // ===== SPECIFICATIONS =====
        public List<ProductSpecificationRequest> Specifications { get; set; } = new();

        // ===== IMAGES =====
        public List<ProductImageRequest> Images { get; set; } = new();
    }

    // -------- VARIATION --------
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

    // -------- SPECIFICATION --------
    public class ProductSpecificationRequest
    {
        public string SpecKey { get; set; } = null!;
        public string SpecValue { get; set; } = null!;
    }

    // -------- IMAGE --------
    public class ProductImageRequest
    {
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
    }
}
