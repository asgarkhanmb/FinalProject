using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Admin.ContactSettings;
using Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Controllers.Admin
{
    public class ContactSettingController :BaseController
    {
        private readonly IContactSettingService _contactSettingService;

        public ContactSettingController(IContactSettingService contactSettingService)
        {
            _contactSettingService = contactSettingService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ContactSettingCreateDto request)
        {
            await _contactSettingService.CreateAsync(request);
            return CreatedAtAction(nameof(Create), new { Response = "Data Successfully Created" });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery][Required] int id)
        {
            await _contactSettingService.DeleteAsync(id);
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] ContactSettingEditDto request)
        {
            await _contactSettingService.EditAsync(id, request);
            return Ok();
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
