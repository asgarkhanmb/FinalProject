using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Ui.Baskets;
using Service.Helpers.Exceptions;
using Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;


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
            if (basket == null) throw new NotFoundException("User not found");
            return Ok(basket);
        }
        [HttpPost]
        public async Task<IActionResult> AddBasket([FromQuery]BasketCreateDto basketCreateDto)
        {
            await _basketService.AddBasketAsync(basketCreateDto);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity([FromQuery]BasketCreateDto basketCreateDto)
        {
            await _basketService.IncreaseQuantityAsync(basketCreateDto);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity([FromQuery]BasketCreateDto basketCreateDto)
        {
            await _basketService.DecreaseQuantityAsync(basketCreateDto);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteProductFromBasket(int productId, string userId)
        {
            await _basketService.DeleteProductFromBasketAsync(productId, userId);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBaskets()
        {
            var baskets = await _basketService.GetAllBasketsAsync();
            return Ok(baskets);
        }
    }
}
