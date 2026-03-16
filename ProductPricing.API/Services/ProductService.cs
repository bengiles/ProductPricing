using ProductPricing.API.Data;
using ProductPricing.Models;

namespace ProductPricing.API.Services
{
    public interface IProductService
    {
        List<ProductSummaryDto> GetAllProducts();
        ProductHistoryDto? GetProductById(int id);
        ApplyDiscountResponse? ApplyDiscount(int id, decimal discountPercentage);
        UpdatePriceResponse? UpdatePrice(int id, decimal newPrice);
    }

    public class ProductService(IProductRepository repository) : IProductService
    {
        public List<ProductSummaryDto> GetAllProducts() =>
            repository.GetAll().Select(p => new ProductSummaryDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                LastUpdated = p.LastUpdated
            }).ToList();

        public ProductHistoryDto? GetProductById(int id)
        {
            var product = repository.GetById(id);
            if (product is null)
                return null;

            return new ProductHistoryDto
            {
                Id = product.Id,
                Name = product.Name,
                PriceHistory = product.PriceHistory
            };
        }

        public ApplyDiscountResponse? ApplyDiscount(int id, decimal discountPercentage)
        {
            var product = repository.GetById(id);
            if (product is null)
                return null;

            if (discountPercentage < 0 || discountPercentage > 100)
                throw new ArgumentOutOfRangeException(nameof(discountPercentage), "Discount must be between 0 and 100.");

            product.DiscountPercentage = discountPercentage;

            return new ApplyDiscountResponse
            {
                Id = product.Id,
                Name = product.Name,
                OriginalPrice = product.Price,
                DiscountedPrice = product.DiscountedPrice ?? product.Price,
            };
        }

        public UpdatePriceResponse? UpdatePrice(int id, decimal newPrice)
        {
            var product = repository.GetById(id);
            if (product is null)
                return null;

            if (newPrice <= 0)
                throw new ArgumentOutOfRangeException(nameof(newPrice), "Price must be positive.");

            repository.AddPriceHistoryEntry(product, new PriceHistoryEntry { Price = newPrice, Date = DateTime.UtcNow });

            return new UpdatePriceResponse
            {
                Id = product.Id,
                Name = product.Name,
                NewPrice = product.Price,
                LastUpdated = product.LastUpdated,
            };
        }
    }
}
