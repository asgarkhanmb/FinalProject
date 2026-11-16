using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.Admin
{
    [Authorize("Admin")]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _orderService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return Ok(await _orderService.GetByIdAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeStatus([FromRoute] int id, [FromQuery] string status)
        {
            await _orderService.ChangeStatusAsync(id, status);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            return Ok(await _orderService.GetStatsAsync());
        }
    }
}
