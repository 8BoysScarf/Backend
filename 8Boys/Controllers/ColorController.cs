using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Services;
using Microsoft.AspNetCore.Mvc;

namespace _8Boys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorController : ControllerBase
    {
        private readonly ColorService _colorService;

        public ColorController(ColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _colorService.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddColorDTO dto)
        {
            await _colorService.CreateAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateColorDTO dto)
        {
            if (id != dto.Id) return BadRequest();
            await _colorService.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var count = await _colorService.DeleteAsync(id);
            if (count == 0) return NotFound();
            return NoContent();
        }
    }
}
