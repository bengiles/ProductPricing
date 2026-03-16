namespace ProductPricing.Models
{
    public class ProductHistoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<PriceHistoryEntry> PriceHistory { get; set; } = [];
    }
}