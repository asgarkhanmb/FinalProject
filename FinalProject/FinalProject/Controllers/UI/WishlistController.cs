using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Ui.Wishlists;
using Service.Helpers.Exceptions;
using Service.Services;
using Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Controllers.UI
{

    public class WishlistController :BaseController
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
            await _wishlistService.AddWishlistAsync(wishlistDto);
            return CreatedAtAction(nameof(GetWishlistByUserId), new { userId = wishlistDto.AppUserId }, wishlistDto);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteProductFromWishlist(int productId, string userId)
        {
            await _wishlistService.DeleteProductFromWishlistAsync(productId, userId);
            return Ok();
        }
     
    }
}
