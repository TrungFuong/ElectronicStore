using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductRepository : IGenericsRepository<Product>
    {
        //dùng cho product 
        Task<Product?> GetByIdAsync(string productId);
        Task<IEnumerable<Product>> GetAllWithIncludesAsync();
    }
}
