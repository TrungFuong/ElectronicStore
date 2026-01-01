using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CategoryRepository
        : GenericRepository<Category>, ICategoryRepository
    {
        private readonly DBContext _context;
        public CategoryRepository(DBContext context) : base(context) {
            _context = context;
        }
    }
}
