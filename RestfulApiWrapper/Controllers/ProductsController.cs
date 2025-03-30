using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RestfulApiWrapper.Attributes;
using RestfulApiWrapper.Models;
using RestfulApiWrapper.Services;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace RestfulApiWrapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ApiControllerBase
    {
        private readonly IRestfulApiService _apiService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IRestfulApiService apiService, ILogger<ProductsController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ApiObject>>> GetProducts([FromQuery] string nameFilter = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            ValidatePagination(pageNumber, pageSize);

            try
            {
                var result = await _apiService.GetObjectsAsync(nameFilter, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve products");
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiObject>> GetProduct([ValidProductId] string id)
        {
            try
            {
                var product = await _apiService.GetObjectByIdAsync(id);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve product {ProductId}", id);
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiObject>> CreateProduct([FromBody] CreateObjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationError(ModelState);
            }

            try
            {
                var createdProduct = await _apiService.CreateObjectAsync(request);
                return CreatedAtAction(
                    nameof(GetProduct),
                    new { id = createdProduct.Id },
                    createdProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create product");
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiObject>> UpdateProduct([ValidProductId] string id, [FromBody] UpdateObjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationError(ModelState);
            }

            try
            {
                var existingProduct = await _apiService.GetObjectByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                var updatedProduct = new ApiObject
                {
                    Id = id,
                    Name = request.Name ?? existingProduct.Name,
                    Data = request.Data ?? existingProduct.Data
                };

                var result = await _apiService.UpdateObjectAsync(id, updatedProduct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update product {ProductId}", id);
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([ValidProductId] string id)
        {
            try
            {
                var existingProduct = await _apiService.GetObjectByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                var success = await _apiService.DeleteObjectAsync(id);
                return success ? NoContent() : StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product {ProductId}", id);
                throw;
            }
        }
    }
}
