using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.UI
{
    public class CategoryController :BaseController
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _categoryService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return Ok(await _categoryService.GetByIdAsync(id));
        }
    }
}
