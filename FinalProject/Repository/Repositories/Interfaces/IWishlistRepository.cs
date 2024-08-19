using Domain.Entities;


namespace Repository.Repositories.Interfaces
{
    public interface IWishlistRepository : IBaseRepository<Wishlist>
    {
        Task<Wishlist> GetByUserIdAsync(string userId);
        Task<Wishlist> GetByIdAsync(int id);
        Task AddAsync(Wishlist wishlist);
        Task UpdateAsync(Wishlist wishlist);
        Task DeleteAsync(int id);
        Task<bool> UserExistsAsync(string userId);
        Task<bool> ProductExistAsync(int productId);

    }
}
