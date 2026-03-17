using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductPricing.API.Services;
using ProductPricing.Models;

namespace ProductPricing.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    [AllowAnonymous]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _productService.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be a positive integer.");

            var product = _productService.GetProductById(id);
            if (product is null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost("{id:int}/apply-discount")]
        public IActionResult ApplyDiscount(int id, [FromBody] ApplyDiscountRequest request)
        {
            if (id <= 0)
                return BadRequest("Id must be a positive integer.");

            try
            {
                var result = _productService.ApplyDiscount(id, request.DiscountPercentage);
                if (result is null)
                    return NotFound();
                return Ok(result);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}/update-price")]
        public IActionResult UpdatePrice(int id, [FromBody] UpdatePriceRequest request)
        {
            if (id <= 0)
                return BadRequest("Id must be a positive integer.");

            try
            {
                var result = _productService.UpdatePrice(id, request.NewPrice);
                if (result is null)
                    return NotFound();
                return Ok(result);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
