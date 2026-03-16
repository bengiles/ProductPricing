namespace ProductPricing.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price => PriceHistory.Count > 0 ? PriceHistory[0].Price : 0;
        public DateTime LastUpdated => PriceHistory.Count > 0 ? PriceHistory[0].Date : default;
        public decimal DiscountPercentage { get; set; }
        public decimal? DiscountedPrice => DiscountPercentage > 0
            ? Math.Round(Price - (Price * DiscountPercentage / 100), 2)
            : null;
        public List<PriceHistoryEntry> PriceHistory { get; set; } = [];
    }

    public class PriceHistoryEntry
    {
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
    }
}
