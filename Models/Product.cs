namespace ProductPricing.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<PriceHistoryEntry> PriceHistory { get; set; } = [];
}

public class PriceHistoryEntry
{
    public decimal Price { get; set; }
    public DateTime Date { get; set; }
}