using Service.DTOs.Admin.Wishlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<WishlistDto> GetWishlistByUserIdAsync(string userId);
        Task AddWishlistAsync(WishlistDto wishlistDto);
        Task DeleteWishlistAsync(int id);
        Task DeleteProductFromWishList(int productId, int wishListId);

    }
}
