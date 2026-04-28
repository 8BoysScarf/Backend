using _8Boys.Models;
using _8Boys.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _8Boys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShippingController : ControllerBase
    {
        private readonly ShippingService _shippingService;

        public ShippingController(ShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        [HttpGet("price")]
        [Authorize]
        public async Task<IActionResult> GetMyShippingPrice()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var price = await _shippingService.GetShippingPriceForUserAsync(userId);
            return Ok(new { ShippingPrice = price });
        }

        [HttpGet("cities")]
        [Authorize]
        public async Task<IActionResult> GetAllCityPrices()
        {
            var list = await _shippingService.GetAllCityPricesAsync();
            return Ok(list);
        }

        [HttpPut("cities")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetCityPrice([FromQuery] string city, [FromQuery] decimal price)
        {
            if (string.IsNullOrWhiteSpace(city)) return BadRequest();
            await _shippingService.SetCityPriceAsync(city, price);
            return NoContent();
        }

        [HttpGet("cities/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCity(int id)
        {
            var item = await _shippingService.GetCityAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpDelete("cities/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            await _shippingService.DeleteCityAsync(id);
            return NoContent();
        }
    }
}
