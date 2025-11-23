using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppMVC.Models
{
    [Table("Categories", Schema = "admin")]
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = [];
    }
}
