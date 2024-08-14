using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Helpers.Exceptions;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.Admin
{
    [Authorize("Admin")]
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
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWishlistByUserId(string userId)
        {
            var wishlist = await _wishlistService.GetWishlistByUserIdAsync(userId);
            if (wishlist == null) throw new NotFoundException("User not found");
            return Ok(wishlist);
        }
    }
}
