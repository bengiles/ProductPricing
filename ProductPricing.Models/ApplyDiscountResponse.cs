namespace ProductPricing.Models
{
    public class ApplyDiscountResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
    }
}
