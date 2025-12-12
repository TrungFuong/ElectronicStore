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
    public class ProductRepository<Product> : IGenericsRepository<Product> where Product : class
    {
        private readonly DBContext _context;

        public GenericRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Set<Product>().ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync(Expression<Func<Product, bool>> expression, params Expression<Func<Product, object>>[] includeProperties)
        {
            IQueryable<Product> query = _context.Set<Product>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            if (expression != null)
            {
                query = query.Where(expression);
            }
            return await query.ToListAsync();
        }

        public async Task<(IEnumerable<Product> items, int totalCount)> GetAllAsync(int page = 1, Expression<Func<Product, bool>> filter = null, Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null, string includeProperties = "", Expression<Func<Product, bool>>? prioritizeCondition = null, int pageSize = 10)
        {
            IQueryable<Product> query = _context.Set<Product>();
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            List<Product> prioritizedItems = new List<Product>();
            List<Product> nonPrioritizedItems = new List<Product>();

            if (prioritizeCondition != null)
            {
                var activeExpression = Expression.Lambda<Func<Product, bool>>(
                   Expression.Not(Expression.Property(prioritizeCondition.Parameters[0], "IsActive")),
                   prioritizeCondition.Parameters);

                var prioritizedQuery = query.Where(prioritizeCondition).Where(activeExpression);
                var nonPrioritizedQuery = query.Where(Expression.Lambda<Func<Product, bool>>(Expression.Not(prioritizeCondition.Body), prioritizeCondition.Parameters));

                if (filter != null)
                {
                    nonPrioritizedQuery = nonPrioritizedQuery.Where(filter);
                }

                if (orderBy != null)
                {
                    nonPrioritizedQuery = orderBy(nonPrioritizedQuery);
                }

                prioritizedItems = await prioritizedQuery.ToListAsync();
                nonPrioritizedItems = await nonPrioritizedQuery.ToListAsync();
            }
            else
            {
                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (orderBy != null)
                {
                    query = orderBy(query);
                }
                nonPrioritizedItems = await query.ToListAsync();
            }

            var items = prioritizedItems.Concat(nonPrioritizedItems).ToList();
            var totalCount = items.Count();

            var paginatedItems = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return (paginatedItems, totalCount);
        }

        public async Task<Product> GetAsync(Expression<Func<Product, bool>> expression)
        {
            IQueryable<Product> query = _context.Set<Product>();
            if (expression != null)
            {
                query = query.Where(expression);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Product> GetAsync(Expression<Func<Product, bool>> expression, params Expression<Func<Product, object>>[] includeProperties)
        {
            IQueryable<Product> query = _context.Set<Product>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            if (expression != null)
            {
                query = query.Where(expression);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(Product entity)
        {
            await _context.Set<Product>().AddAsync(entity);
        }

        public Task AddRangeAsync(IEnumerable<Product> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(Product entity)
        {
            _context.Set<Product>().Update(entity);
        }

        public void Delete(Product entity)
        {
            _context.Set<Product>().Remove(entity);
        }

        public void SoftDelete(Product entity)
        {
            PropertyInfo propertyInfo = entity.GetType().GetProperty("IsActive");
            propertyInfo.SetValue(entity, false);
            _context.Set<Product>().Update(entity);
        }

        public void RemoveRange(IEnumerable<Product> entities)
        {
            _context.Set<Product>().RemoveRange(entities);
        }

        public async Task<int> CountAsync(Expression<Func<Product, bool>> expression = null)
        {
            IQueryable<Product> query = _context.Set<Product>();
            if (expression != null)
            {
                query = query.Where(expression);
            }
            return await query.CountAsync();
        }
    }
}
