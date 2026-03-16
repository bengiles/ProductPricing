using NUnit.Framework;
using ProductPricing.Services;

namespace ProductPricing.Tests;

[TestFixture]
public class ProductServiceTests
{
    private ProductService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new ProductService();
    }

    [Test]
    public void GetAllProducts_ReturnsSeededProducts()
    {
        var products = _service.GetAllProducts();

        Assert.That(products, Has.Count.GreaterThanOrEqualTo(2));
    }

    [Test]
    public void GetProductById_ExistingId_ReturnsProduct()
    {
        var product = _service.GetProductById(1);

        Assert.That(product, Is.Not.Null);
        Assert.That(product!.Name, Is.EqualTo("Product A"));
    }

    [Test]
    public void GetProductById_NonExistingId_ReturnsNull()
    {
        var product = _service.GetProductById(999);

        Assert.That(product, Is.Null);
    }

    [Test]
    public void ApplyDiscount_ValidDiscount_ReducesPrice()
    {
        var originalPrice = _service.GetProductById(1)!.Price;

        var result = _service.ApplyDiscount(1, 10);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Value.OriginalPrice, Is.EqualTo(originalPrice));
        Assert.That(result.Value.DiscountedPrice, Is.EqualTo(originalPrice * 0.9m));
    }

    [Test]
    public void ApplyDiscount_ZeroPercent_PriceUnchanged()
    {
        var originalPrice = _service.GetProductById(1)!.Price;

        var result = _service.ApplyDiscount(1, 0);

        Assert.That(result!.Value.DiscountedPrice, Is.EqualTo(originalPrice));
    }

    [Test]
    public void ApplyDiscount_100Percent_PriceBecomesZero()
    {
        var result = _service.ApplyDiscount(1, 100);

        Assert.That(result!.Value.DiscountedPrice, Is.EqualTo(0m));
    }

    [Test]
    public void ApplyDiscount_NegativeDiscount_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.ApplyDiscount(1, -5));
    }

    [Test]
    public void ApplyDiscount_DiscountOver100_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.ApplyDiscount(1, 150));
    }

    [Test]
    public void ApplyDiscount_NonExistingProduct_ReturnsNull()
    {
        var result = _service.ApplyDiscount(999, 10);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ApplyDiscount_AddsToPriceHistory()
    {
        var product = _service.GetProductById(1)!;
        var historyCountBefore = product.PriceHistory.Count;

        _service.ApplyDiscount(1, 10);

        Assert.That(product.PriceHistory, Has.Count.EqualTo(historyCountBefore + 1));
    }

    [Test]
    public void UpdatePrice_ValidPrice_UpdatesProduct()
    {
        var result = _service.UpdatePrice(1, 150.0m);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Price, Is.EqualTo(150.0m));
    }

    [Test]
    public void UpdatePrice_ZeroPrice_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.UpdatePrice(1, 0));
    }

    [Test]
    public void UpdatePrice_NegativePrice_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.UpdatePrice(1, -50));
    }

    [Test]
    public void UpdatePrice_NonExistingProduct_ReturnsNull()
    {
        var result = _service.UpdatePrice(999, 100);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void UpdatePrice_AddsToPriceHistory()
    {
        var product = _service.GetProductById(2)!;
        var historyCountBefore = product.PriceHistory.Count;

        _service.UpdatePrice(2, 250.0m);

        Assert.That(product.PriceHistory, Has.Count.EqualTo(historyCountBefore + 1));
    }

    [Test]
    public void UpdatePrice_UpdatesLastUpdated()
    {
        var before = DateTime.UtcNow;

        _service.UpdatePrice(1, 300.0m);

        var product = _service.GetProductById(1)!;
        Assert.That(product.LastUpdated, Is.GreaterThanOrEqualTo(before));
    }
}