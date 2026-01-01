using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    // Unit of Work Pattern
    // Quản lý các repository và đảm bảo tính nhất quán khi thực hiện các thao tác với cơ sở dữ liệu
    // Đóng gói các thao tác liên quan đến nhiều repository vào một đơn vị công việc duy nhất
    // Giúp kiểm soát giao dịch (transaction) và đảm bảo rằng tất cả các thay đổi được thực hiện hoặc không có thay đổi nào được thực hiện
    // Giúp giảm sự phụ thuộc giữa các repository và làm cho mã nguồn dễ bảo trì hơn
    // IUnitOfWork: Định nghĩa các repository và phương thức để lưu thay đổi
    // UnitOfWork: Triển khai IUnitOfWork và quản lý các repository
    // Sử dụng DBContext để tương tác với cơ sở dữ liệu
    // Cung cấp các phương thức Commit và CommitAsync để lưu thay đổi vào cơ sở dữ liệu
    //Thay vì tạo mới từng repository trong service, ta sẽ sử dụng UnitOfWork để quản lý tất cả các repository
    //chỉ cần tạo 1 instance của UnitOfWork và sử dụng nó để truy cập các repository cần thiết
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DBContext _context;
        private IAccountRepository _accountRepository;
        private IBrandRepository _brandRepository;
        private ICategoryRepository _category_repository;
        //private ICustomerRepository _customer_repository;
        //private IProductRepository _product_repository;
        private IRefreshTokenRepository _refreshTokenRepository;
        private IProductRepository _productRepository;

        // new repositories
        private IDiscountRepository _discountRepository;
        private IOrderRepository _orderRepository;
        private IStaffRepository _staffRepository;

        private ICartRepository? _cartRepository;
        private ICartItemRepository? _cartItemRepository;
        private IGenericsRepository<ProductSpecification>? _productSpecificationRepository;
        private IGenericsRepository<ProductImage>? _productImageRepository;
        private IGenericsRepository<VariationAttribute>? _variationAttribute_repository;
        private IGenericsRepository<VariationOption>? _variation_option_repository;
        private IGenericsRepository<ProductVariation>? _productVariationRepository;

        // orders

        // new repositories
        private ICustomerRepository _customerRepository;

        public UnitOfWork(DBContext context)
        {
            _context = context;
        }

        public IAccountRepository AccountRepository
            => _accountRepository ??= new AccountRepository(_context);

        public IRefreshTokenRepository RefreshTokenRepository
            => _refreshTokenRepository ??= new RefreshTokenRepository(_context);
        public IBrandRepository BrandRepository
        => _brandRepository ??= new BrandRepository(_context);

        public ICategoryRepository CategoryRepository
        => _category_repository ??= new CategoryRepository(_context);

        public IStaffRepository StaffRepository
            => _staffRepository ??= new StaffRepository(_context);

        public IProductRepository ProductRepository
        => _productRepository ??= new ProductRepository(_context);

        public IGenericsRepository<ProductVariation> ProductVariationRepository
        => _productVariationRepository
           ??= new GenericRepository<ProductVariation>(_context);

        public IGenericsRepository<ProductSpecification> ProductSpecificationRepository
        => _productSpecificationRepository
           ??= new GenericRepository<ProductSpecification>(_context);
        public IGenericsRepository<ProductImage> ProductImageRepository
        => _productImageRepository
           ??= new GenericRepository<ProductImage>(_context);
        public IGenericsRepository<VariationAttribute> VariationAttributeRepository 
        => _variationAttribute_repository
           ??= new GenericRepository<VariationAttribute>(_context);
        public IGenericsRepository<VariationOption> VariationOptionRepository
        => _variation_option_repository
           ??= new GenericRepository<VariationOption>(_context);

        public IOrderRepository OrderRepository
        => _orderRepository ??= new OrderRepository(_context);

        public IDiscountRepository DiscountRepository
            => _discountRepository ??= new DiscountRepository(_context);

        public ICustomerRepository CustomerRepository
            => _customerRepository ??= new CustomerRepository(_context);

        
        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await action();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        
        public ICartRepository CartRepository
    => _cartRepository ??= new CartRepository(_context);

        public ICartItemRepository CartItemRepository
            => _cartItemRepository ??= new CartItemRepository(_context);

        
        public int Commit()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
