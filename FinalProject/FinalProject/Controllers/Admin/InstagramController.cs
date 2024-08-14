using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Admin.Instagrams;
using Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Controllers.Admin
{
    [Authorize("Admin")]
    public class InstagramController :BaseController
    {
        private readonly IInstagramService _instagramService;
        public InstagramController(IInstagramService instagramService)
        {
            _instagramService = instagramService;   
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] InstagramCreateDto request)
        {
            await _instagramService.CreateAsync(request);
            return CreatedAtAction(nameof(Create), new { Response = "Data Successfully Created" });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery][Required] int id)
        {
            await _instagramService.DeleteAsync(id);
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] InstagramEditDto request)
        {
            await _instagramService.EditAsync(id, request);
            return Ok();
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
