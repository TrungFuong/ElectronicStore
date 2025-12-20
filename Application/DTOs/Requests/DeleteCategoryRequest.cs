using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class DeleteCategoryRequest
    {
        [Required]
        public string CategoryId { get; set; }
    }
}
