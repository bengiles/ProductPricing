namespace ProductPricing.Models
{
    public class ProductSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}