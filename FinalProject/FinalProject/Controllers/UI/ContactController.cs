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
        public async Task<IActionResult> Create([FromBody] ContactCreateDto request)
        {
            await _contactService.CreateAsync(request);
            return CreatedAtAction(nameof(Create), new { Response = "Successfully added" });
        }
    }
}
