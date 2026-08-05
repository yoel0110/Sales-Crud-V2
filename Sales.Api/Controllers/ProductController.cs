using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales.Api.utils;
using Sales.Application.Dtos;
using Sales.Application.Interfaces;
using Sales.Application.Types;

namespace Sales.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/v1/product")]
    public class ProductController: ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetsAll([FromQuery] int filter, [FromQuery] decimal minPrice, [FromQuery] decimal maxPrice, [FromQuery] int price, [FromQuery] int lenght = 10, [FromQuery] string category = "Toys")
        {
            var products = await _productService.GetProducts(minPrice, maxPrice, lenght, (decimal) price, category,  filter: (FILTER) filter);
            return Ok(ApiResponse<List<ProductDto>>.SuccessFul(data: products));
        }

        [HttpDelete("removeby")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> RemoveBy([FromQuery] int id)
        {
            var product  = await _productService.DeleteById(id);
            var deleted = new ProductDto()
            {
                ProductId = product.ProductId,
                Category = new CategoryDto
                {
                    Id = product.CategoryId,
                    Name = product.Category.CategoryName
                },
                Price = product.Price,
                ProductName = product.ProductName,
                Stock = product.Stock
            };
            return Ok(ApiResponse<ProductDto>.SuccessFul(data: deleted, statusCode: 200));
        }
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<String>>> Add([FromBody] ProductDto product)
        {
            if (string.IsNullOrWhiteSpace(product.ProductName) || product.Price < 0 || product.Stock < 0)
            {
                return BadRequest(ApiResponse<String>.Failure("Datos invalidos", statusCode: 400));
            }

            var id = await _productService.Create(product);
            return Ok(ApiResponse<String>.SuccessFul(data: id, message: "Ok, created"));
        }

        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Update([FromBody] ProductDto product)
        {
            if (string.IsNullOrWhiteSpace(product.ProductName) || product.Price < 0 || product.Stock < 0)
            {
                return BadRequest(ApiResponse<ProductDto>.Failure("Datos invalidos", statusCode: 400));
            }

            var updatedProduct = await _productService.Update(product);
            var productDto = new ProductDto()
            {
                ProductId = updatedProduct.ProductId,
                Category = new CategoryDto
                {
                    Id = updatedProduct.CategoryId,
                    Name = updatedProduct.Category.CategoryName
                },
                Price = updatedProduct.Price,
                ProductName = updatedProduct.ProductName,
                Stock = updatedProduct.Stock
            };
            return Ok(ApiResponse<ProductDto>.SuccessFul(data: productDto, message: "Ok, updated"));
        }
    }
}
