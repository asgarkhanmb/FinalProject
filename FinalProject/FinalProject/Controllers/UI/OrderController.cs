using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Ui.Orders;
using Service.Helpers.Exceptions;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.UI
{
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // İstifadəçi Id-si client-dən deyil, JWT tokendən götürülür
        private string CurrentUserId =>
            User.FindFirst("uid")?.Value ?? throw new UnauthorizeException("Yenidən daxil olun.");

        /// <summary>Səbətdən sifariş yaradır və Stripe Checkout linkini qaytarır.</summary>
        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto request)
        {
            return Ok(await _orderService.CheckoutAsync(CurrentUserId, request));
        }

        /// <summary>Stripe-dan qayıtdıqdan sonra ödənişi yoxlayır.</summary>
        [HttpPost]
        public async Task<IActionResult> Confirm([FromQuery] string sessionId)
        {
            return Ok(await _orderService.ConfirmPaymentAsync(CurrentUserId, sessionId));
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            return Ok(await _orderService.GetMyOrdersAsync(CurrentUserId));
        }
    }
}
