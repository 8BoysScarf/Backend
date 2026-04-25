using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Services;
using Microsoft.AspNetCore.Mvc;

namespace _8Boys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BadgeController : ControllerBase
    {
        private readonly BadgeService _badgeService;

        public BadgeController(BadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _badgeService.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddBadgeDTO dto)
        {
            await _badgeService.CreateAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBadgeDTO dto)
        {
            if (id != dto.Id) return BadRequest();
            await _badgeService.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var count = await _badgeService.DeleteAsync(id);
            if (count == 0) return NotFound();
            return NoContent();
        }
    }
}
