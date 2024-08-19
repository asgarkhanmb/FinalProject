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
            await _context.Baskets.AddAsync(basket);
        }

        public async Task<Basket> GetByUserIdAsync(string userId)
        {
            return await _context.Baskets
        .Include(b => b.BasketProducts)
        .FirstOrDefaultAsync(b => b.AppUserId == userId);
        }

        public async Task<bool> ProductExistAsync(int productId)
        {
            return await _context.Products.AnyAsync(p => p.Id == productId);

        }

        public void Remove(BasketProduct basketProduct)
        {
            _context.BasketProducts.Remove(basketProduct);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }
    }
}
