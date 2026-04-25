using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Services;
using Microsoft.AspNetCore.Mvc;

namespace _8Boys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var product = await _productService.GetDetailsDtoAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpGet("cards")]
        public async Task<IActionResult> GetCards(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? colorId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? badgeId = null)
        {
            var result = await _productService.SearchPagedAsync(page, pageSize, name, categoryId, colorId, minPrice, maxPrice, badgeId);
            return Ok(result);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AddProductDTO dto)
        {

            await _productService.CreateAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDTO dto)
        {
            await _productService.UpdateAsync(id , dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var count = await _productService.DeleteAsync(id);
            if (count == 0) return NotFound();
            return NoContent();
        }
    }
}