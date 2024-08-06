using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.UI
{
    public class InstagramController :BaseController
    {
        private readonly IInstagramService _instagramService;
        public InstagramController(IInstagramService instagramService)
        {
            _instagramService = instagramService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _instagramService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return Ok(await _instagramService.GetByIdAsync(id));
        }
    }
}
