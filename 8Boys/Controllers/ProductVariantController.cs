using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Services;
using Microsoft.AspNetCore.Mvc;

namespace _8Boys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductVariantController : ControllerBase
    {
        private readonly ProductVariantService _variantService;

        public ProductVariantController(ProductVariantService variantService)
        {
            _variantService = variantService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var v = await _variantService.GetByIdAsync(id);
            if (v == null) return NotFound();
            return Ok(v);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] AddVariantDTO dto)
        {
            var id = await _variantService.AddVariantAsync(dto);
            return CreatedAtAction(nameof(Get), new { id }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateVariantDTO dto)
        {
            if (id != dto.Id) return BadRequest();
            await _variantService.UpdateVariantAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var count = await _variantService.RemoveVariantAsync(id);
            if (count == 0) return NotFound();
            return NoContent();
        }
    }
}
