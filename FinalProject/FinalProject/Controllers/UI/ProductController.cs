using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.UI
{
    public class ProductController :BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _productService.GetAllAsync());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return Ok(await _productService.GetByIdAsync(id));
        }
        [HttpGet]
        public async Task<IActionResult> GetPaginateDatas([FromQuery] int page = 1, [FromQuery] int take = 2)
        {
            return Ok(await _productService.GetPaginateDataAsync(page, take));
        }
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            return Ok(await _productService.Search(name));
        }
        [HttpGet]
        public async Task<IActionResult> SortBy([FromQuery] string sortKey, bool isDescending)
        {
            return Ok(await _productService.SortBy(sortKey, isDescending));
        }
    }
}
