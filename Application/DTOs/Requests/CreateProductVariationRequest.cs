using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class CreateProductVariationRequest
    {
        [Required]
        public string ProductId { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public List<CreateVariationOptionRequest> Options { get; set; } = new();
    }

    
}
