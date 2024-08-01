using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Ui.Contacts;
using Service.Services;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.UI
{
    public class ContactController :BaseController
    {
        private readonly IContactService _contactService;


        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] ContactCreateDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _contactService.CreateAsync(request);
            return CreatedAtAction(nameof(Create), new { Response = "Data Successfully Created" });
        }
    }
}
