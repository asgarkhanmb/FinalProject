using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Admin.Subscribes;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.UI
{
    public class SubscribeController :BaseController
    {
        private readonly ISubscribeService _subscribeService;

        public SubscribeController(ISubscribeService subscribeService)
        {
            _subscribeService = subscribeService;
        }
        [HttpPost]
        public async Task<IActionResult> AddSubscribe([FromQuery] SubscribeCreateDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _subscribeService.AddSubscribeAsync(request);
            return CreatedAtAction(nameof(AddSubscribe), new { Response = "Data Successfully Created" });
        }
    }
}
