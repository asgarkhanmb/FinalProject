using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.UI
{
    public class ContactSettingController :BaseController
    {
        private readonly IContactSettingService _contactSettingService;

        public ContactSettingController(IContactSettingService contactSettingService)
        {
            _contactSettingService = contactSettingService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _contactSettingService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return Ok(await _contactSettingService.GetByIdAsync(id));
        }
    }
}
