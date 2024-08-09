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
    public class BasketRepository :BaseRepository<Basket>,IBasketRepository
    {

        public BasketRepository(AppDbContext context) : base(context) { }

        public async Task AddAsync(Basket basket)
        {
            _context.Baskets.Add(basket);
        }

        public async Task DeleteAsync(int id)
        {
           var basket = await GetByIdAsync(id);
           if(basket != null)
            {
                _context.Baskets.Remove(basket);
            }

        }

        public async Task<Basket> GetByIdAsync(int id)
        {
            return await _context.Baskets.Include(m => m.BasketProducts)
                                         .ThenInclude(m => m.Product)
                                         .ThenInclude(m => m.ProductImages)
                                         .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Basket> GetByUserIdAsync(string userId)
        {
            return await _context.Baskets.Include(m => m.BasketProducts)
                                         .ThenInclude(m => m.Product)
                                         .ThenInclude(m => m.ProductImages)
                                         .FirstOrDefaultAsync(m => m.AppUserId == userId);
        }

        public async Task UpdateAsync(Basket basket)
        {
            _context.Baskets.Update(basket);
        }
    }
}
