using System.ComponentModel.DataAnnotations;

namespace ProductPricing.Models
{
    public class ApplyDiscountRequest
    {
        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public decimal DiscountPercentage { get; set; }
    }
}
