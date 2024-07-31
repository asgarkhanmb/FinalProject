using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Admin.Testimonials;
using Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Controllers.Admin
{
    public class TestimonialController :BaseController
    {
        private readonly ITestimonialService _testimonialService;

        public TestimonialController(ITestimonialService testimonialService)
        {
            _testimonialService = testimonialService;   
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] TestimonialCreateDto request)
        {
            await _testimonialService.CreateAsync(request);
            return CreatedAtAction(nameof(Create), request);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery][Required] int id)
        {
            await _testimonialService.DeleteAsync(id);
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] TestimonialEditDto request)
        {
            await _testimonialService.EditAsync(id, request);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _testimonialService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return Ok(await _testimonialService.GetByIdAsync(id));
        }
        [HttpGet]
        public async Task<IActionResult> GetPaginateDatas([FromQuery] int page = 1, [FromQuery] int take = 2)
        {
            return Ok(await _testimonialService.GetPaginateDataAsync(page, take));
        }
    }
}
