using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class UpdateProductRequest
    {
        // ===== PRODUCT =====
        
        public string? ProductId { get; set; } 
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public string? CategoryId { get; set; }
        public string? BrandId { get; set; }
        // Allow toggling active state in update
        public bool? IsActive { get; set; }

        // ===== VARIATIONS =====
        public List<UpdateProductVariationRequest> Variations { get; set; } = new();

        // ===== SPECIFICATIONS =====
        public List<UpdateProductSpecificationRequest> Specifications { get; set; } = new();

        // ===== IMAGES =====
        public List<UpdateProductImageRequest> Images { get; set; } = new();
    }

    // -------- VARIATION --------
    public class UpdateProductVariationRequest
    {
        public string? VariationId { get; set; }   // null = thêm mới
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsDeleted { get; set; }

        public List<UpdateVariationOptionRequest> Options { get; set; } = new();
    }

    public class UpdateVariationOptionRequest
    {
        public string? OptionId { get; set; }      // null = thêm mới
        public int AttributeId { get; set; }
        public string OptionValue { get; set; } = null!;
        public bool IsDeleted { get; set; }
    }

    // -------- SPECIFICATION --------
    public class UpdateProductSpecificationRequest
    {
        public string? SpecificationId { get; set; } // null = thêm mới
        public string SpecKey { get; set; } = null!;
        public string SpecValue { get; set; } = null!;
        public bool IsDeleted { get; set; }
    }

    // -------- IMAGE --------
    public class UpdateProductImageRequest
    {
        public string? ImageId { get; set; }       // null = thêm mới
        public string ImageUrl { get; set; } 
        public bool IsMain { get; set; }
        public bool IsDeleted { get; set; }
    }
}
