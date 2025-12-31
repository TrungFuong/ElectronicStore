using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class ProductSearchRequest
    {
        public List<string>? CategoryIds { get; set; }
        public List<string>? BrandIds { get; set; }
        public string? Keyword { get; set; }
        public string? Status { get; set; } // all | active | inactive

    }
}
