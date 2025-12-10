namespace MinimalAPI.Models.Dto
{
    public class ProductCategoryDto
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public long Quantity { get; set; }
        public bool Status { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
