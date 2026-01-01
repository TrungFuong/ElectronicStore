using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class AttributeResponse
    {
        public int AttributeId { get; set; }
        public string Name { get; set; } = null!;
    }
}
