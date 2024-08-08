using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories.Interfaces;


namespace Repository.Repositories
{
    public class ProductRepository :BaseRepository<Product>,IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Product>> FilterAsync(string categoryName)
        {
            IEnumerable<Product> query = await _context.Products
                .Include(m => m.Category)
                .Include(m => m.ProductImages)
                .ToListAsync();

            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(p => p.Category.Name.ToLower() == categoryName.ToLower());
            }
            return query.ToList();
        }

        public async Task<IEnumerable<Product>> GetPaginateDataAsync(int page, int take)
        {
            return await _entities.Skip((page - 1) * take).Take(take).ToListAsync();
        }

        public async Task<IEnumerable<Product>> SortBy(string sortKey, bool isDescending)
        {
            if (_entities == null)
            {
                throw new InvalidOperationException("The entities collection is not initialized.");
            }
            var query = _entities.AsQueryable();
            var normalizedSortKey = sortKey?.ToLowerInvariant();
            switch (normalizedSortKey)
            {
                case "name":
                    query = isDescending ? query.OrderByDescending(m => m.Name) : query.OrderBy(m => m.Name);
                    break;

                case "price":
                    query = isDescending ? query.OrderByDescending(m => m.Price) : query.OrderBy(m => m.Price);
                    break;

                default:
                    throw new ArgumentException($"Invalid sort key: {sortKey}", nameof(sortKey));
            }
            return await query.ToListAsync();
        }
    }
}
