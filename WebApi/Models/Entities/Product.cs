using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models.Entities
{
    [Table("Products", Schema = "admin")]
    public class Product
    {
        [Key]
        public long ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public long Quantity { get; set; }

        [Required]
        public bool Status { get; set; }

        [Required]
        [ForeignKey("Category")]
        public long CategoryId { get; set; }

        // virtual để hỗ trợ lazy loading
        public virtual Category? Category { get; set; }
    }
}
