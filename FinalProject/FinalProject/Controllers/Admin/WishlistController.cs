using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.Admin
{
    public class WishlistController :BaseController
    {
        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWishlist()
        {
            var wishlist = await _wishlistService.GetAllWishlistsAsync();
            return Ok(wishlist);
        }
    }
}
