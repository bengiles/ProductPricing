using Moq;
using NUnit.Framework.Legacy;
using ProductPricing.API.Data;
using ProductPricing.API.Services;
using ProductPricing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPricing.API.UnitTests.ServiceTests
{
    [TestFixture]
    public class ProductServiceTests
    {
        #region GetAllProducts Tests
        [Test]
        public void GetAllProducts_Returns_All_Products()
        {
            // Arrange
            var productHistory1 = new List<PriceHistoryEntry>
            {
                new PriceHistoryEntry { Price = 10m, Date = DateTime.UtcNow.AddDays(-10) },
                new PriceHistoryEntry { Price = 12m, Date = DateTime.UtcNow.AddDays(-5) }
            };
            var productHistory2 = new List<PriceHistoryEntry>
            {
                new PriceHistoryEntry { Price = 20m, Date = DateTime.UtcNow.AddDays(-8) },
                new PriceHistoryEntry { Price = 22m, Date = DateTime.UtcNow.AddDays(-3) }
            };
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", PriceHistory = productHistory1 },
                new Product { Id = 2, Name = "Product 2", PriceHistory = productHistory2 }
            };
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetAll()).Returns(products);
            var service = new ProductService(repository.Object);
            // Act
            var result = service.GetAllProducts();
            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.Count);
            ClassicAssert.AreEqual(1, result[0].Id);
            ClassicAssert.AreEqual("Product 1", result[0].Name);
            ClassicAssert.AreEqual(10m, result[0].Price);
            ClassicAssert.AreEqual(2, result[1].Id);
            ClassicAssert.AreEqual("Product 2", result[1].Name);
            ClassicAssert.AreEqual(20m, result[1].Price);
            repository.Verify(r => r.GetAll(), Times.Once);
        }

        [Test]
        public void GetAllProducts_EmptyRepository_Returns_EmptyList()
        {
            // Arrange
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetAll()).Returns(new List<Product>());
            var service = new ProductService(repository.Object);
            // Act
            var result = service.GetAllProducts();
            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(0, result?.Count);
            repository.Verify(r => r.GetAll(), Times.Once);
        }

        #endregion

        #region GetByProductId Tests
        [Test]
        public void GetProductById_ProductExists_Returns_ProductHistory()
        {
            // Arrange
            int testId = 1;
            var priceHistory = new List<PriceHistoryEntry>
            {
                new PriceHistoryEntry { Price = 10m, Date = DateTime.UtcNow.AddDays(-10) },
                new PriceHistoryEntry { Price = 12m, Date = DateTime.UtcNow.AddDays(-5) }
            };
            var product = new Product
            {
                Id = testId,
                Name = "Test Product",
                PriceHistory = priceHistory
            };
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetById(testId)).Returns(product);
            var service = new ProductService(repository.Object);
            // Act
            var result = service.GetProductById(testId);
            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(testId, result.Id);
            ClassicAssert.AreEqual("Test Product", result.Name);
            ClassicAssert.AreEqual(2, result.PriceHistory.Count);
            ClassicAssert.AreEqual(10m, result.PriceHistory[0].Price);
            ClassicAssert.AreEqual(12m, result.PriceHistory[1].Price);
            repository.Verify(r => r.GetById(testId), Times.Once);
        }

        [Test]
        public void GetProductById_ProductNotFound_Returns_Null()
        {
            // Arrange
            int testId = 999; // This ID does not exist in the repository
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetById(testId)).Returns((Product)null!);
            var service = new ProductService(repository.Object);
            // Act
            var result = service.GetProductById(testId);
            // Assert
            ClassicAssert.IsNull(result);
            repository.Verify(r => r.GetById(testId), Times.Once);
        }
        #endregion         

        #region ApplyDiscount Tests

        [Test]
        public void ApplyDiscount_Product_NotFound_Returns_Null()
        {
            // Arrange
            int testId = 999; // This ID does not exist in the repository
            decimal discountPercentage = 10;
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetById(testId)).Returns((Product)null!);
            var service = new ProductService(repository.Object);
            // Act
            var result = service.ApplyDiscount(testId, discountPercentage);
            // Assert
            ClassicAssert.IsNull(result);
            repository.Verify(r => r.GetById(testId), Times.Once);
        }

        [Test]
        public void ApplyDiscount_InvalidDiscountPercentage_Throws_Exception()
        {
            // Arrange
            int testId = 1;
            decimal invalidDiscountPercentage = -5; // Invalid discount percentage
            var product = new Product
            {
                Id = testId,
                Name = "Test Product",               
            };
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetById(testId)).Returns(product);
            var service = new ProductService(repository.Object);
            // Act & Assert
            ClassicAssert.Throws<ArgumentOutOfRangeException>(() => service.ApplyDiscount(testId, invalidDiscountPercentage));
            ClassicAssert.Throws<ArgumentOutOfRangeException>(() => service.ApplyDiscount(testId, 150)); // Another invalid case
            repository.Verify(r => r.GetById(testId), Times.Exactly(2));
        }

        [Test]
        public void ApplyDiscount_ValidInput_Returns_DiscountedPrice()
        {
            // Arrange
            int testId = 1;
            decimal discountPercentage = 20; // Valid discount percentage
            var priceHistory = new List<PriceHistoryEntry>
            {
                new PriceHistoryEntry { Price = 100m, Date = DateTime.UtcNow }
            };
            var product = new Product
            {
                Id = testId,
                Name = "Test Product",
                PriceHistory = priceHistory
            };
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetById(testId)).Returns(product);
            var service = new ProductService(repository.Object);
            // Act
            var result = service.ApplyDiscount(testId, discountPercentage);
            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(testId, result.Id);
            ClassicAssert.AreEqual("Test Product", result.Name);
            ClassicAssert.AreEqual(100m, result.OriginalPrice);
            ClassicAssert.AreEqual(80m, result.DiscountedPrice); // 20% off of 100 is 80
            repository.Verify(r => r.GetById(testId), Times.Once);
        }
        #endregion

        #region UpdatePrice Tests
        [Test]
        public void UpdatePrice_Product_NotFound_Returns_Null()
        {
            // Arrange
            int testId = 999; // This ID does not exist in the repository
            decimal newPrice = 50m;
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetById(testId)).Returns((Product)null!);
            var service = new ProductService(repository.Object);
            // Act
            var result = service.UpdatePrice(testId, newPrice);
            // Assert
            ClassicAssert.IsNull(result);
            repository.Verify(r => r.GetById(testId), Times.Once);
        }

        [Test]
        public void UpdatePrice_ValidInput_Returns_UpdatedPrice()
        {
            // Arrange
            int testId = 1;
            decimal newPrice = 50m; // Valid new price
            var priceHistory = new List<PriceHistoryEntry>
            {
                new PriceHistoryEntry { Price = 100m, Date = DateTime.UtcNow }
            };
            var product = new Product
            {
                Id = testId,
                Name = "Test Product",
                PriceHistory = priceHistory
            };
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetById(testId)).Returns(product);
            repository.Setup(r => r.AddPriceHistoryEntry(It.IsAny<Product>(), It.IsAny<PriceHistoryEntry>()))
                .Callback<Product, PriceHistoryEntry>((p, entry) => p.PriceHistory.Insert(0, entry));
            var service = new ProductService(repository.Object);
            // Act
            var result = service.UpdatePrice(testId, newPrice);
            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(testId, result.Id);
            ClassicAssert.AreEqual("Test Product", result.Name);
            Assert.That(priceHistory.FirstOrDefault().Price, Is.EqualTo(newPrice));
            Assert.That(result.NewPrice, Is.EqualTo(newPrice));
            repository.Verify(r => r.GetById(testId), Times.Once);
        }

        [Test]
        public void UpdatePrice_InvalidNewPrice_Throws_Exception()
        {
            // Arrange
            int testId = 1;
            decimal invalidNewPrice = -10m; // Invalid new price
            var product = new Product
            {
                Id = testId,
                Name = "Test Product",
            };
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetById(testId)).Returns(product);
            var service = new ProductService(repository.Object);
            // Act & Assert
            ClassicAssert.Throws<ArgumentOutOfRangeException>(() => service.UpdatePrice(testId, invalidNewPrice));
            ClassicAssert.Throws<ArgumentOutOfRangeException>(() => service.UpdatePrice(testId, 0)); // Another invalid case
            repository.Verify(r => r.GetById(testId), Times.Exactly(2));
        }

        [Test]
        public void UpdatePrice_Repository_AddPriceHistoryEntry_Called()
        {
            // Arrange
            int testId = 1;
            decimal newPrice = 50m; // Valid new price
            var priceHistory = new List<PriceHistoryEntry>
            {
                new PriceHistoryEntry { Price = 100m, Date = DateTime.UtcNow }
            };
            var product = new Product
            {
                Id = testId,
                Name = "Test Product",
                PriceHistory = priceHistory
            };
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetById(testId)).Returns(product);
            var service = new ProductService(repository.Object);
            // Act
            var result = service.UpdatePrice(testId, newPrice);
            // Assert
            ClassicAssert.IsNotNull(result);
            repository.Verify(r => r.AddPriceHistoryEntry(product, It.Is<PriceHistoryEntry>(entry => entry.Price == newPrice)), Times.Once);
        }
        #endregion


    }
}
