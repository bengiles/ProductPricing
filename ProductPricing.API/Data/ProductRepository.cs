using ProductPricing.Models;

namespace ProductPricing.API.Data
{
    public interface IProductRepository
    {
        List<Product> GetAll();
        Product? GetById(int id);
        void AddPriceHistoryEntry(Product product, PriceHistoryEntry entry);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly List<Product> _products =
        [
            new Product
            {
                Id = 1,
                Name = "Product A",
                PriceHistory =
                [
                    new PriceHistoryEntry { Price = 120.0m, Date = new DateTime(2024, 9, 1) },
                    new PriceHistoryEntry { Price = 110.0m, Date = new DateTime(2024, 8, 15) },
                    new PriceHistoryEntry { Price = 100.0m, Date = new DateTime(2024, 8, 10) }
                ]
            },
            new Product
            {
                Id = 2,
                Name = "Product B",
                PriceHistory =
                [
                    new PriceHistoryEntry { Price = 220.0m, Date = new DateTime(2024, 9, 5) },
                    new PriceHistoryEntry { Price = 200.0m, Date = new DateTime(2024, 8, 20) }
                ]
            }
        ];

        public List<Product> GetAll() => _products;

        public Product? GetById(int id) =>
            _products.Where(p => p.Id == id).OrderByDescending(p => p.LastUpdated).FirstOrDefault();

        public void AddPriceHistoryEntry(Product product, PriceHistoryEntry entry) =>
            product.PriceHistory.Insert(0, entry);
    }
}