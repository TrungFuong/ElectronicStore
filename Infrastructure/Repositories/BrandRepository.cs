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
        public async Task<string> GenerateNewBrandIdAsync()
        {
            var lastId = await _context.Brands
                .Where(b => b.BrandId.StartsWith(Prefixes.BRAND_ID_PREFIX))
                .OrderByDescending(b => b.BrandId)
                .Select(b => b.BrandId)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(lastId))
                return Prefixes.BRAND_ID_PREFIX + string.Format(Prefixes.ID_FORMAT, 1);

            var numberPart = lastId.Substring(Prefixes.BRAND_ID_PREFIX.Length);

            if (!int.TryParse(numberPart, out int number))
                throw new Exception("BrandId format is invalid: " + lastId);

            return Prefixes.BRAND_ID_PREFIX + string.Format(Prefixes.ID_FORMAT, number + 1);
        }

    }
}