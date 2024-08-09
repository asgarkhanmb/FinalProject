using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Admin.Wishlists;
using Service.Helpers.Exceptions;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.Admin
{
    public class WishlistController : BaseController
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;

        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWishlistByUserId(string userId)
        {
            var wishlist = await _wishlistService.GetWishlistByUserIdAsync(userId);
            if (wishlist == null) throw new NotFoundException("User not found");
            return Ok(wishlist);
        }

        [HttpPost]
        public async Task<IActionResult> AddWishlist([FromQuery] WishlistDto wishlistDto)
        {
            var wishlist = await _wishlistService.GetWishlistByUserIdAsync(wishlistDto.ToString());
            if (wishlist == null) throw new NotFoundException("User not found");
            await _wishlistService.AddWishlistAsync(wishlistDto);
            return CreatedAtAction(nameof(GetWishlistByUserId), new { userId = wishlistDto.AppUserId }, wishlistDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWishlist(int id)
        {
            if (id == null) throw new NotFoundException("Not Found");
            await _wishlistService.DeleteWishlistAsync(id);
            return Ok();
        }
        [HttpDelete("DeleteProductFromWishlist/{id}")]
        public async Task<IActionResult> DeleteProductFromWishlist(int id, int productId)
        {
            await _wishlistService.DeleteProductFromWishList(productId, id);
            return NoContent();
        }
    }
}
