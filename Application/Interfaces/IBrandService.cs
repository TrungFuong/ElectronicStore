using Application.DTOs.Requests;
using Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBrandService
    {
        Task CreateAsync(CreateBrandRequest request);
        Task<IEnumerable<BrandResponse>> GetAllAsync();

        Task<bool> UpdateAsync(UpdateBrandRequest request);
        Task<bool> DeleteAsync(DeleteBrandRequest request);

    }
}
