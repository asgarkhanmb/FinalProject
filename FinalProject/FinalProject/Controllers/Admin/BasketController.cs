using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Admin.Baskets;
using Service.Services.Interfaces;


namespace FinalProject.Controllers.Admin
{
    public class BasketController :BaseController
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;

        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBasketByUserId(string userId)
        {
            var basket = await _basketService.GetBasketByUserIdAsync(userId);
            if (basket == null)
            {
                return NotFound();
            }
            return Ok(basket);
        }

        [HttpPost]
        public async Task<IActionResult> AddBasket([FromQuery] BasketDto basketDto)
        {
            await _basketService.AddBasketAsync(basketDto);
            return CreatedAtAction(nameof(GetBasketByUserId), new { userId = basketDto.AppUserId }, basketDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBasket(int id)
        {
            await _basketService.DeleteBasketAsync(id);
            return NoContent();
        }
        [HttpDelete("DeleteProductFromBasket/{id}")]
        public async Task<IActionResult> DeleteProductFromBasket(int id, int productId)
        {
            await _basketService.DeleteProductFromBasket(productId, id);
            return NoContent();
        }
    }
}
