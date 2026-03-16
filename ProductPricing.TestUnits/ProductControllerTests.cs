using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductPricing.API.Controllers;
using ProductPricing.API.Services;
using ProductPricing.Models;

namespace ProductPricing.TestUnits
{
    [TestFixture]
    public class ProductControllerTests
    {
        private Mock<IProductService> _serviceMock;
        private ProductController _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IProductService>();
            _controller = new ProductController(_serviceMock.Object);
        }

        #region GET /api/products

        [Test]
        public void GetAll_ReturnsOkWithProducts()
        {
            var products = new List<ProductSummaryDto>
            {
                new() { Id = 1, Name = "Product A", Price = 120.0m, LastUpdated = new DateTime(2024, 9, 26, 12, 34, 56) },
                new() { Id = 2, Name = "Product B", Price = 200.0m, LastUpdated = new DateTime(2024, 9, 25, 10, 12, 34) }
            };
            _serviceMock.Setup(s => s.GetAllProducts()).Returns(products);

            var result = _controller.GetAll() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            var returned = result.Value as List<ProductSummaryDto>;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned!, Has.Count.EqualTo(2));
            Assert.That(returned![0].Name, Is.EqualTo("Product A"));
            Assert.That(returned[1].Name, Is.EqualTo("Product B"));
        }

        [Test]
        public void GetAll_ReturnsOkWithEmptyList_WhenNoProducts()
        {
            _serviceMock.Setup(s => s.GetAllProducts()).Returns([]);

            var result = _controller.GetAll() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            var returned = result.Value as List<ProductSummaryDto>;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned!, Is.Empty);
        }

        #endregion

        #region GET /api/products/{id}

        [Test]
        public void GetById_ReturnsOkWithPriceHistory_WhenProductExists()
        {
            var history = new ProductHistoryDto
            {
                Id = 1,
                Name = "Product A",
                PriceHistory =
                [
                    new() { Price = 120.0m, Date = new DateTime(2024, 9, 1) },
                    new() { Price = 110.0m, Date = new DateTime(2024, 8, 15) },
                    new() { Price = 100.0m, Date = new DateTime(2024, 8, 10) }
                ]
            };
            _serviceMock.Setup(s => s.GetProductById(1)).Returns(history);

            var result = _controller.GetById(1) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            var returned = result.Value as ProductHistoryDto;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned!.Id, Is.EqualTo(1));
            Assert.That(returned.Name, Is.EqualTo("Product A"));
            Assert.That(returned.PriceHistory, Has.Count.EqualTo(3));
            Assert.That(returned.PriceHistory[0].Price, Is.EqualTo(120.0m));
        }

        [Test]
        public void GetById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            _serviceMock.Setup(s => s.GetProductById(999)).Returns((ProductHistoryDto?)null);

            var result = _controller.GetById(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        #endregion

        #region POST /api/products/{id}/apply-discount

        [Test]
        public void ApplyDiscount_ReturnsOkWithDiscountedPrice_WhenProductExists()
        {
            var response = new ApplyDiscountResponse
            {
                Id = 1,
                Name = "Product A",
                OriginalPrice = 100.0m,
                DiscountedPrice = 90.0m
            };
            _serviceMock.Setup(s => s.ApplyDiscount(1, 10m)).Returns(response);

            var request = new ApplyDiscountRequest { DiscountPercentage = 10m };
            var result = _controller.ApplyDiscount(1, request) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            var returned = result.Value as ApplyDiscountResponse;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned!.OriginalPrice, Is.EqualTo(100.0m));
            Assert.That(returned.DiscountedPrice, Is.EqualTo(90.0m));
        }

        [Test]
        public void ApplyDiscount_ReturnsNotFound_WhenProductDoesNotExist()
        {
            _serviceMock.Setup(s => s.ApplyDiscount(999, 10m)).Returns((ApplyDiscountResponse?)null);

            var request = new ApplyDiscountRequest { DiscountPercentage = 10m };
            var result = _controller.ApplyDiscount(999, request);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void ApplyDiscount_CalculatesCorrectly_ForZeroPercent()
        {
            var response = new ApplyDiscountResponse
            {
                Id = 1,
                Name = "Product A",
                OriginalPrice = 100.0m,
                DiscountedPrice = 100.0m
            };
            _serviceMock.Setup(s => s.ApplyDiscount(1, 0m)).Returns(response);

            var request = new ApplyDiscountRequest { DiscountPercentage = 0m };
            var result = _controller.ApplyDiscount(1, request) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var returned = result!.Value as ApplyDiscountResponse;
            Assert.That(returned!.DiscountedPrice, Is.EqualTo(returned.OriginalPrice));
        }

        [Test]
        public void ApplyDiscount_CalculatesCorrectly_For100Percent()
        {
            var response = new ApplyDiscountResponse
            {
                Id = 1,
                Name = "Product A",
                OriginalPrice = 100.0m,
                DiscountedPrice = 0.0m
            };
            _serviceMock.Setup(s => s.ApplyDiscount(1, 100m)).Returns(response);

            var request = new ApplyDiscountRequest { DiscountPercentage = 100m };
            var result = _controller.ApplyDiscount(1, request) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var returned = result!.Value as ApplyDiscountResponse;
            Assert.That(returned!.DiscountedPrice, Is.EqualTo(0.0m));
        }

        #endregion

        #region PUT /api/products/{id}/update-price

        [Test]
        public void UpdatePrice_ReturnsOkWithUpdatedPrice_WhenProductExists()
        {
            var response = new UpdatePriceResponse
            {
                Id = 1,
                Name = "Product A",
                NewPrice = 150.0m,
                LastUpdated = new DateTime(2024, 9, 26, 14, 0, 0)
            };
            _serviceMock.Setup(s => s.UpdatePrice(1, 150.0m)).Returns(response);

            var request = new UpdatePriceRequest { NewPrice = 150.0m };
            var result = _controller.UpdatePrice(1, request) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            var returned = result.Value as UpdatePriceResponse;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned!.Id, Is.EqualTo(1));
            Assert.That(returned.NewPrice, Is.EqualTo(150.0m));
            Assert.That(returned.LastUpdated, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public void UpdatePrice_ReturnsNotFound_WhenProductDoesNotExist()
        {
            _serviceMock.Setup(s => s.UpdatePrice(999, 150.0m)).Returns((UpdatePriceResponse?)null);

            var request = new UpdatePriceRequest { NewPrice = 150.0m };
            var result = _controller.UpdatePrice(999, request);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        #endregion

        #region Service invocation verification

        [Test]
        public void GetAll_CallsServiceExactlyOnce()
        {
            _serviceMock.Setup(s => s.GetAllProducts()).Returns([]);

            _controller.GetAll();

            _serviceMock.Verify(s => s.GetAllProducts(), Times.Once);
        }

        [Test]
        public void GetById_CallsServiceWithCorrectId()
        {
            _serviceMock.Setup(s => s.GetProductById(42)).Returns((ProductHistoryDto?)null);

            _controller.GetById(42);

            _serviceMock.Verify(s => s.GetProductById(42), Times.Once);
        }

        [Test]
        public void ApplyDiscount_CallsServiceWithCorrectParameters()
        {
            _serviceMock.Setup(s => s.ApplyDiscount(1, 15m)).Returns((ApplyDiscountResponse?)null);

            _controller.ApplyDiscount(1, new ApplyDiscountRequest { DiscountPercentage = 15m });

            _serviceMock.Verify(s => s.ApplyDiscount(1, 15m), Times.Once);
        }

        [Test]
        public void UpdatePrice_CallsServiceWithCorrectParameters()
        {
            _serviceMock.Setup(s => s.UpdatePrice(1, 250m)).Returns((UpdatePriceResponse?)null);

            _controller.UpdatePrice(1, new UpdatePriceRequest { NewPrice = 250m });

            _serviceMock.Verify(s => s.UpdatePrice(1, 250m), Times.Once);
        }

        #endregion
    }
}
