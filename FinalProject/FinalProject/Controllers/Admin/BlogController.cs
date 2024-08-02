using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Admin.Blogs;
using Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Controllers.Admin
{
    public class BlogController:BaseController
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] BlogCreateDto request)
        {
            await _blogService.CreateAsync(request);
            return CreatedAtAction(nameof(Create), new { Response = "Data Successfully Created" });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery][Required] int id)
        {
            await _blogService.DeleteAsync(id);
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] BlogEditDto request)
        {
            await _blogService.EditAsync(id, request);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _blogService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return Ok(await _blogService.GetByIdAsync(id));
        }
        [HttpGet]
        public async Task<IActionResult> GetPaginateDatas([FromQuery] int page = 1, [FromQuery] int take = 2)
        {
            return Ok(await _blogService.GetPaginateDataAsync(page, take));
        }
    }
}
