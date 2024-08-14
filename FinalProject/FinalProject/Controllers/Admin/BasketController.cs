﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Helpers.Exceptions;
using Service.Services.Interfaces;



namespace FinalProject.Controllers.Admin
{
    [Authorize("Admin")]
    public class BasketController :BaseController
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBaskets()
        {
            var baskets = await _basketService.GetAllBasketsAsync();
            return Ok(baskets);
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBasketByUserId(string userId)
        {
            var basket = await _basketService.GetBasketByUserIdAsync(userId);
            if (basket == null) throw new NotFoundException("User not found");
            return Ok(basket);
        }
    }
}
