using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class CreateProductImageRequest
    {
        public string? ProductId { get; set; }
        public List<CreateProductImageItem> Images { get; set; } = new();
    }

    public class CreateProductImageItem
    {
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
    }

}
