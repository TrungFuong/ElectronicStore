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
            public decimal ProductPrice { get; set; }
            public int StockQuantity { get; set; }
            public string ProductDescription { get; set; } = null!;
            public string CategoryId { get; set; } = null!;
            public string BrandId { get; set; } = null!;
        }
    



}
