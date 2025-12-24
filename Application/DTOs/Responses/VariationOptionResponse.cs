using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class VariationOptionResponse
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
