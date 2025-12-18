using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models.Dto;
using WebApi.Models.Entities;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(long id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            _context.Products.Remove(new Product { ProductId = id });
            await _context.SaveChangesAsync();
        }

        public bool Exists(long id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        public async Task<IEnumerable<ProductCategoryDto>> GetAllWithCategoryAsync()
        {
            return await _context.Products.Select(p => new ProductCategoryDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price,
                Description = p.Description,
                Quantity = p.Quantity,
                Status = p.Status,
                CategoryId = p.Category!.CategoryId,
                CategoryName = p.Category!.CategoryName
            }).ToListAsync();
        }

        public async Task<ProductCategoryDto?> GetByIdWithCategoryAsync(long id)
        {
            return await _context.Products.Where(p => p.ProductId == id).Select(p => new ProductCategoryDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price,
                Description = p.Description,
                Quantity = p.Quantity,
                Status = p.Status,
                CategoryId = p.Category!.CategoryId,
                CategoryName = p.Category!.CategoryName
            }).FirstOrDefaultAsync();
        }
    }
}
