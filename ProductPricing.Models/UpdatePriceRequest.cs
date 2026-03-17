using System.ComponentModel.DataAnnotations;

namespace ProductPricing.Models
{
    public class UpdatePriceRequest
    {
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be positive.")]
        public decimal NewPrice { get; set; }
    }
}
