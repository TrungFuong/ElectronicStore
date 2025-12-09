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
        //private IBrandRepository _brandRepository;
        //private ICategoryRepository _categoryRepository;
        //private ICustomerRepository _customerRepository;
        //private IProductRepository _productRepository;
        private IRefreshTokenRepository _refreshTokenRepository;

        public UnitOfWork(DBContext context)
        {
            _context = context;
        }

        public IAccountRepository AccountRepository
            => _accountRepository ??= new AccountRepository(_context);

        public IRefreshTokenRepository RefreshTokenRepository
            => _refreshTokenRepository ??= new RefreshTokenRepository(_context);

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
