using _8Boys.DTOs;
using _8Boys.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _8Boys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly AddressService _addressService;

        public AddressController(AddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var list = await _addressService.GetUserAddressesAsync(userId);
            return Ok(list);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] AddAddressDTO dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var id = await _addressService.AddAddressAsync(userId, dto);
            return CreatedAtAction(nameof(GetAll), new { id }, null);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAddressDTO dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            if (id != dto.Id) return BadRequest();
            await _addressService.UpdateAddressAsync(userId, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var count = await _addressService.DeleteAddressAsync(userId, id);
            if (count == 0) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/default")]
        [Authorize]
        public async Task<IActionResult> SetDefault(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            await _addressService.SetDefaultAddressAsync(userId, id);
            return NoContent();
        }
    }
}
