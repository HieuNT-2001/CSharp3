using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Dto;
using WebApi.Models.Entities;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Retrieves a list of all products along with their associated category information.
        /// </summary>
        /// <remarks>This method handles HTTP GET requests for the products endpoint. The returned list
        /// includes both product details and their corresponding categories. The response has an HTTP 200 status code
        /// on success.</remarks>
        /// <returns>An <see cref="ActionResult{T}"/> containing a collection of <see cref="ProductCategoryDto"/> objects. The
        /// collection will be empty if no products are available.</returns>
        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCategoryDto>>> GetProducts()
        {
            var products = await _productService.GetAllWithCategoryAsync();
            return Ok(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategoryDto>> GetProduct(long id)
        {
            var product = await _productService.GetByIdWithCategoryAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, Product product)
        {
            if (!_productService.Exists(id)) return NotFound();
            await _productService.UpdateAsync(product);
            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            await _productService.CreateAsync(product);
            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            if (!_productService.Exists(id)) return NotFound();
            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}
