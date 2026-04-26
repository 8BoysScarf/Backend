using _8Boys.DTOs;
using _8Boys.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _8Boys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create-from-cart")]
        [Authorize]
        public async Task<IActionResult> CreateFromCart([FromBody] CheckoutDTO dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var id = await _orderService.CreateOrderFromCartAsync(userId, dto.AddressId);
            return Ok(new { OrderId = id });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDetails(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var details = await _orderService.GetOrderDetailsAsync(id, userId);
            if (details == null) return NotFound();
            return Ok(details);
        }

        [HttpGet("my-orders")]
        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }
    }
}
