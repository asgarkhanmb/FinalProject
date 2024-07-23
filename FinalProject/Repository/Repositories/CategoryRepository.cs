
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories
{
    public class CategoryRepository :BaseRepository<Category>,ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }


        public async Task<IEnumerable<Category>> GetPaginateDataAsync(int page, int take)
        {
            return await _entities.Skip((page - 1) * take).Take(take).ToListAsync();
        }
    }
}
