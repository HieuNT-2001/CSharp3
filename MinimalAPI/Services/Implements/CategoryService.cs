using Microsoft.EntityFrameworkCore;
using MinimalAPI.Data;
using MinimalAPI.Models.Entities;
using MinimalAPI.Services.Interfaces;

namespace MinimalAPI.Services.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(long id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public bool Exists(long id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

    }
}
