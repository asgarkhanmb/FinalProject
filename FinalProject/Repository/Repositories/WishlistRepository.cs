using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories.Interfaces;


namespace Repository.Repositories
{
    public class WishlistRepository : BaseRepository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(AppDbContext context) : base(context) { }

        public async Task<Wishlist> GetByUserIdAsync(string userId)
        {
            return await _context.Wishlists
      .Include(w => w.WishlistProducts)
      .ThenInclude(wp => wp.Product)
      .ThenInclude(p => p.ProductImages) // Ensure ProductImages is included
      .FirstOrDefaultAsync(w => w.AppUserId == userId);
        }

        public async Task<Wishlist> GetByIdAsync(int id)
        {
            return await _context.Wishlists
                .Include(w => w.WishlistProducts)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task AddAsync(Wishlist wishlist)
        {
            await _context.Wishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Wishlist wishlist)
        {
            _context.Wishlists.Update(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var wishlist = await GetByIdAsync(id);
            if (wishlist != null)
            {
                _context.Wishlists.Remove(wishlist);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<bool> ProductExistAsync(int productId)
        {
            return await _context.Products.AnyAsync(p => p.Id == productId);
        }
    }
}
