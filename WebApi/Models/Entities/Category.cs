using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models.Entities
{
    [Table("Categories", Schema = "admin")]
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        // virtual để hỗ trợ lazy loading
        public virtual ICollection<Product> Products { get; set; } = [];
    }
}
