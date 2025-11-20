namespace WebAppMVC.Models
{
    public class Product
    {
        public long ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public long Quantity { get; set; }
        public bool Status { get; set; }

    }
}
