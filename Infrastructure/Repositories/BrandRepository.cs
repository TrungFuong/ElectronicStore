using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class BrandRepository
        : GenericRepository<Brand>, IBrandRepository
    {
        private readonly DBContext _context;
        public BrandRepository(DBContext context) : base(context)
        {
            _context = context;
        }

    }
}