using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class StaffRepository : GenericRepository<Staff>, IStaffRepository
    {
        private readonly DBContext _context;

        public StaffRepository(DBContext context) : base(context)
        {
            _context = context;
        }
    }
}
