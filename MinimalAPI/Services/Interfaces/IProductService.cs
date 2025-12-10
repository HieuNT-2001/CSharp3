using MinimalAPI.Models.Dto;
using MinimalAPI.Models.Entities;

namespace MinimalAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(long id);
        Task CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(long id);
        bool Exists(long id);
        Task<IEnumerable<ProductCategoryDto>> GetAllWithCategoryAsync();
        Task<ProductCategoryDto?> GetByIdWithCategoryAsync(long id);
    }
}
