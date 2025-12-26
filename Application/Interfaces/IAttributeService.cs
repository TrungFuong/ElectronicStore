using Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAttributeService
    {
        Task<IEnumerable<AttributeResponse>> GetAllAsync();
    }
}
