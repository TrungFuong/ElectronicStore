using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class DeleteDiscountRequest
    {
        [Required]
        public string DiscountId { get; set; }
    }
}
