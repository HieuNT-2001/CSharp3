using Microsoft.EntityFrameworkCore;
using MinimalAPI.Models.Entities;
using MinimalAPI.Services.Interfaces;
namespace MinimalAPI.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Product").WithTags(nameof(Product));

        group.MapGet("/", async (IProductService productService) =>
        {
            var products = await productService.GetAllWithCategoryAsync();
            return TypedResults.Ok(products);
        })
        .WithName("GetAllProducts");

        group.MapGet("/{productid}", async Task<IResult> (long productid, IProductService productService) =>
        {
            var product = await productService.GetByIdWithCategoryAsync(productid);
            if (product == null) return TypedResults.NotFound($"product with ID {productid} not found.");
            return TypedResults.Ok(product);
        })
        .WithName("GetProductById");

        group.MapPut("/{productid}", async Task<IResult> (long productid, Product product, IProductService productService) =>
        {
            if (!productService.Exists(productid)) return TypedResults.NotFound();
            await productService.UpdateAsync(product);
            return TypedResults.NoContent();
        })
        .WithName("UpdateProduct");

        group.MapPost("/", async (Product product, IProductService productService) =>
        {
            await productService.CreateAsync(product);
            return TypedResults.Created($"/api/Product/{product.ProductId}", product);
        })
        .WithName("CreateProduct");

        group.MapDelete("/{productid}", async Task<IResult> (long productid, IProductService productService) =>
        {
            if (!productService.Exists(productid)) return TypedResults.NotFound();
            await productService.DeleteAsync(productid);
            return TypedResults.NoContent();
        })
        .WithName("DeleteProduct");
    }
}
