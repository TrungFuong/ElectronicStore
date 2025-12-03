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
        //IBrandRepository Brands { get; }
        //ICategoryRepository Categories { get; }
        //ICustomerRepository Customers { get; }
        //IProductRepository Products { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        Task<int> CommitAsync();
        int Commit();
    }
}
