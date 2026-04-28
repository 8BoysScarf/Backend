using _8Boys.DTOs;
using _8Boys.Models;
using _8Boys.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _8Boys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var items = await _cartService.GetCartItemsDtoAsync(userId);
            return Ok(items);
        }

        [HttpPost("items")]
        [Authorize]
        public async Task<IActionResult> AddItem([FromBody] AddToCartDTO dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _cartService.AddItemAsync(userId, dto.ProductVariantId, dto.Quantity);
            return Ok();
        }

        [HttpDelete("items/{variantId}")]
        [Authorize]
        public async Task<IActionResult> RemoveItem(int variantId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _cartService.RemoveItemAsync(userId, variantId);
            return NoContent();
        }

        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDTO dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var orderId = await _cartService.CheckoutAsync(userId, dto.AddressId);
            return Ok(new { OrderId = orderId });
        }
    }
}
