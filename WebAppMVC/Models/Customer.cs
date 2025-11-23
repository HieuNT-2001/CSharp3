using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppMVC.Models
{
    [Table("Customers", Schema = "admin")]
    public class Customer
    {
        [Key]
        public long CustomerId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Image { get; set; } = string.Empty;
    }
}
