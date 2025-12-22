using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IAccountRepository AccountRepository { get; }
        IBrandRepository BrandRepository { get; }

        ICategoryRepository CategoryRepository { get; }
        //ICustomerRepository Customers { get; }
        IProductRepository ProductRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }

        // Added repositories
        IDiscountRepository DiscountRepository { get; }
        IOrderRepository OrderRepository { get; }

        Task<int> CommitAsync();
        int Commit();
    }
}
