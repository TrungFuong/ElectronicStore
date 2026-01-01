using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataAccess;

namespace Infrastructure.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly DBContext _context;
        public CustomerRepository(DBContext context) : base(context)
        {
            _context = context;
        }
    }
}
