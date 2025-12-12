using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Models.Entities;

namespace WebApi.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        
        // Cấu hình mô hình dữ liệu
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Thêm cấu hình mô hình tại đây nếu cần

            // Cấu hình cho thực thể RefreshToken
            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id); // Khóa chính
                entity.HasIndex(e => e.Token).IsUnique(); // Chỉ mục duy nhất trên trường Token
            });
        }
    }
}
