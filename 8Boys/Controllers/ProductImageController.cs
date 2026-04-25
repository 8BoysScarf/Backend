using _8Boys.DTOs;
using _8Boys.Services;
using Microsoft.AspNetCore.Mvc;

namespace _8Boys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductImageController : ControllerBase
    {
        private readonly ProductImageService _imageService;

        public ProductImageController(ProductImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var images = await _imageService.GetAllAsync();
            return Ok(images);
        }

    }
}
