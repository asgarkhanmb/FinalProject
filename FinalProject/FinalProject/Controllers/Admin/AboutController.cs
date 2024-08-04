using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Admin.Abouts;
using Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Controllers.Admin
{
    [Authorize]
    public class AboutController :BaseController
    {
        private readonly IAboutService _aboutService;

        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AboutCreateDto request)
        {
            await _aboutService.CreateAsync(request);
            return CreatedAtAction(nameof(Create), new { Response = "Data Successfully Created" });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery][Required] int id)
        {
            await _aboutService.DeleteAsync(id);
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] AboutEditDto request)
        {
            await _aboutService.EditAsync(id, request);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _aboutService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return Ok(await _aboutService.GetByIdAsync(id));
        }
    }
}
