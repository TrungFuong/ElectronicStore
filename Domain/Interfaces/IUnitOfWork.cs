using Domain.Entities;
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
        ICustomerRepository CustomerRepository { get; }

        IStaffRepository StaffRepository { get; }
        Task ExecuteInTransactionAsync(Func<Task> action);
        IGenericsRepository<ProductVariation> ProductVariationRepository { get; }
        IGenericsRepository<ProductSpecification> ProductSpecificationRepository { get; }

        IGenericsRepository<ProductImage> ProductImageRepository { get; }

        IGenericsRepository<VariationAttribute> VariationAttributeRepository { get; }
        ICartRepository CartRepository { get; }
        ICartItemRepository CartItemRepository { get; }
        IGenericsRepository<VariationOption> VariationOptionRepository { get; }

        // Orders

        Task<int> CommitAsync();
        int Commit();
    }
}
