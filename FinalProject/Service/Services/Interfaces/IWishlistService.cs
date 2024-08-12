using Domain.Entities;
using Service.DTOs.Ui.Wishlists;
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
        Task DeleteProductFromWishList(int productId, int wishListId);

    }
}
