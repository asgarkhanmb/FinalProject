using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Admin.Socials;
using Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Controllers.Admin
{
    [Authorize("Admin")]
    public class SocialController : BaseController
    {
        private readonly ISocialService _socialService;


        public SocialController(ISocialService socialService)
        {
            _socialService = socialService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SocialCreateDto request)
        {
            await _socialService.CreateAsync(request);
            return CreatedAtAction(nameof(Create), new { Response = "Data Successfully Created" });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery][Required] int id)
        {
            await _socialService.DeleteAsync(id);
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] SocialEditDto request)
        {
            await _socialService.EditAsync(id, request);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _socialService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return Ok(await _socialService.GetByIdAsync(id));
        }
        [HttpGet]
        public async Task<IActionResult> GetPaginateDatas([FromQuery] int page = 1, [FromQuery] int take = 2)
        {
            return Ok(await _socialService.GetPaginateDataAsync(page, take));
        }
    }
}
