using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Controllers.Admin
{
    public class SubscribeController :BaseController
    {
        private readonly ISubscribeService _subscribeService;

        public SubscribeController(ISubscribeService subscribeService)
        {
           _subscribeService = subscribeService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _subscribeService.GetAllAsync());
        }

    }
}
