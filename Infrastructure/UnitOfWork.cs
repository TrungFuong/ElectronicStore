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
