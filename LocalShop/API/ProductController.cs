using LocalShop.Domain.Models;
using LocalShop.Services.DTOs;
using LocalShop.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalShop.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }

        [HttpGet("GetProduct")]
       
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> Create(ProductDto dto)
        {
            await _service.AddProductAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = dto.Id }, dto);
        }
    }
}
