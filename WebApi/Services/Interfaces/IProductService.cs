using WebApi.Models.Dto;
using WebApi.Models.Entities;

namespace WebApi.Services.Interfaces
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
