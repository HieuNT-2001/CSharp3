namespace WebApi.Models.Dto
{
    public class ProductCategoryDto
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public long Quantity { get; set; }
        public bool Status { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}