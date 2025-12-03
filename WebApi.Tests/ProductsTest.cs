using WebApi.Controllers;
using WebApi.Models.Dto;
using WebApi.Services.Interfaces;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Tests
{
    public class ProductsTest
    {
        [Fact]
        public async Task GetProducts_ReturnsListOfProducts()
        {
            // 1. Tạo mock service
            var mockService = new Mock<IProductService>();

            // 2. Setup dữ liệu giả
            mockService.Setup(s => s.GetAllWithCategoryAsync()).ReturnsAsync(new List<ProductCategoryDto>
        {
            new ProductCategoryDto { ProductId = 1, ProductName = "Product A", Price = 10, Description="Desc A", Quantity=5, Status=true, Category="Cat 1" },
            new ProductCategoryDto { ProductId = 2, ProductName = "Product B", Price = 20, Description="Desc B", Quantity=3, Status=true, Category="Cat 2" },
        });

            // 3. Inject mock service vào controller
            var controller = new ProductsController(mockService.Object);

            // 4. Call action
            var result = await controller.GetProducts();

            // 5. Kiểm tra kết quả
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductCategoryDto>>(okResult.Value);
            Assert.Equal(2, products.Count());
            Assert.Equal("Product A", products.First().ProductName);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            var mockService = new Mock<IProductService>();
            mockService.Setup(s => s.GetByIdWithCategoryAsync(99)).ReturnsAsync((ProductCategoryDto?)null);

            var controller = new ProductsController(mockService.Object);

            var result = await controller.GetProduct(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
