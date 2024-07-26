using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ProductRepository :BaseRepository<Product>,IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }


        public async Task<IEnumerable<Product>> GetPaginateDataAsync(int page, int take)
        {
            return await _entities.Skip((page - 1) * take).Take(take).ToListAsync();
        }
    }
}
