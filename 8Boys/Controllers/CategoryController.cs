using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Services;
using Microsoft.AspNetCore.Mvc;

namespace _8Boys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("GetAllNames")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _categoryService.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddCategoryDTO dto)
        {
            
            await _categoryService.CreateAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update( [FromBody] UpdateCategoryDTO dto)
        {
            
            await _categoryService.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var count = await _categoryService.DeleteAsync(id);
            if (count == 0) return NotFound();
            return NoContent();
        }
    }
}
