using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Ui.Wishlists;
using Service.Helpers.Exceptions;
using Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Controllers.UI
{
    [Authorize]
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductFromWishlist([Required] int id, [Required] int productId)
        {
            await _wishlistService.DeleteProductFromWishList(productId, id);
            return Ok();
        }
    }
}
