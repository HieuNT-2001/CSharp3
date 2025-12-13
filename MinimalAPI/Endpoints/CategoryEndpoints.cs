using Microsoft.EntityFrameworkCore;
using MinimalAPI.Models.Entities;
using MinimalAPI.Services.Interfaces;
namespace MinimalAPI.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Category").RequireRateLimiting("fixed").WithTags(nameof(Category));

        group.MapGet("/", async (ICategoryService categoryService) =>
        {
            var categories = await categoryService.GetAllAsync();
            return TypedResults.Ok(categories);
        })
        .WithName("GetAllCategories");

        group.MapGet("/{categoryid}", async Task<IResult> (long categoryid, ICategoryService categoryService) =>
        {
            var product = await categoryService.GetByIdAsync(categoryid);
            if (product == null) return TypedResults.NotFound();
            return TypedResults.Ok(product);
        })
        .WithName("GetCategoryById");

        group.MapPut("/{categoryid}", async Task<IResult> (long categoryid, Category category, ICategoryService categoryService) =>
        {
            if (!categoryService.Exists(categoryid)) return TypedResults.NotFound();
            await categoryService.UpdateAsync(category);
            return TypedResults.NoContent();
        })
        .WithName("UpdateCategory");

        group.MapPost("/", async (Category category, ICategoryService categoryService) =>
        {
            await categoryService.CreateAsync(category);
            return TypedResults.Created($"/api/Category/{category.CategoryId}", category);
        })
        .WithName("CreateCategory");

        group.MapDelete("/{categoryid}", async Task<IResult> (long categoryid, ICategoryService categoryService) =>
        {
            if (!categoryService.Exists(categoryid)) return TypedResults.NotFound();
            await categoryService.DeleteAsync(categoryid);
            return TypedResults.NoContent();
        })
        .WithName("DeleteCategory");
    }
}
