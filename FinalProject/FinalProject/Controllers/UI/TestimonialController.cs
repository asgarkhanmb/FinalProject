using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.UI
{
    public class TestimonialController :BaseController
    {
        private readonly ITestimonialService _testimonialService;

        public TestimonialController(ITestimonialService testimonialService)
        {
            _testimonialService = testimonialService;
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
    }
}
