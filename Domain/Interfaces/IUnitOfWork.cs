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
        IGenericsRepository<ProductVariation> ProductVariationRepository { get; }

        IGenericsRepository<ProductSpecification> ProductSpecificationRepository { get; }

        IGenericsRepository<ProductImage> ProductImageRepository { get; }

        IGenericsRepository<VariationAttribute> VariationAttributeRepository { get; }

        IGenericsRepository<VariationOption> VariationOptionRepository { get; }
        Task<int> CommitAsync();
        int Commit();
    }
}
