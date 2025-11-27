using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using PostgresqlDockerDemo.Application.Abstractions;
using PostgresqlDockerDemo.Application.Services;
using PostgresqlDockerDemo.Domain;

namespace PostgresqlDockerDemo.Tests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private IProductRepository _repo = null!;
        private ProductService _sut = null!; 

        [SetUp]
        public void SetUp()
        {
            _repo = Substitute.For<IProductRepository>();
            _sut = new ProductService(_repo);
        }

        // --- Create() --------------------------------------------------------

        [Test]
        public void Create_ShouldTrimName_AndCallRepository()
        {
            _repo.Create("USB-C Cable", 9.99m).Returns(42);

            var id = _sut.Create("  USB-C Cable  ", 9.99m);

            Assert.That(id, Is.EqualTo(42));
            _repo.Received(1).Create("USB-C Cable", 9.99m);
        }

        [Test]
        public void Create_ShouldThrow_WhenNameIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => _sut.Create("   ", 1m));
            _repo.DidNotReceiveWithAnyArgs().Create(default!, default);
        }

        [Test]
        public void Create_ShouldThrow_WhenNameTooLong()
        {
            var longName = new string('x', 201);
            Assert.Throws<ArgumentException>(() => _sut.Create(longName, 1m));
        }

        [Test]
        public void Create_ShouldThrow_WhenPriceIsNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Create("Mouse", -0.01m));
        }

        // --- Update() --------------------------------------------------------

        [Test]
        public void Update_ShouldThrow_WhenIdIsInvalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Update(0, "X", 1m));
        }

        [Test]
        public void Update_ShouldTrimName_AndCallRepository()
        {
            _repo.Update(1, "USB-C Cable", 12.49m).Returns(true);

            var ok = _sut.Update(1, " USB-C Cable ", 12.49m);

            Assert.That(ok, Is.True);
            _repo.Received(1).Update(1, "USB-C Cable", 12.49m);
        }

        // --- Delete() --------------------------------------------------------
        [Test]
        public void Delete_ShouldCallRepository_WhenIdIsValid()
        {
            _repo.Delete(5).Returns(true);

            var result = _sut.Delete(5);

            Assert.That(result, Is.True);
            _repo.Received(1).Delete(5);
        }

        [Test]
        public void Delete_ShouldThrow_WhenIdIsInvalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Delete(-1));
        }

        // --- ListSortedByPrice() ---------------------------------------------

        [Test]
        public void ListSortedByPrice_ShouldSortAscending()
        {
            _repo.List().Returns(new[]
            {
                new Product(1,"A",10m,DateTimeOffset.UtcNow),
                new Product(2,"B",9.99m,DateTimeOffset.UtcNow),
                new Product(3,"C",29.9m,DateTimeOffset.UtcNow),
            });

            var result = _sut.ListSortedByPrice(true).Select(p => p.Id).ToArray();

            Assert.That(result, Is.EqualTo(new[] { 2L, 1L, 3L }));
        }

        [Test]
        public void ListSortedByPrice_ShouldSortDescending()
        {
            _repo.List().Returns(new[]
            {
                new Product(1,"A",10m,DateTimeOffset.UtcNow),
                new Product(2,"B",9.99m,DateTimeOffset.UtcNow),
                new Product(3,"C",29.9m,DateTimeOffset.UtcNow),
            });

            var result = _sut.ListSortedByPrice(false).Select(p => p.Id).ToArray();

            Assert.That(result, Is.EqualTo(new[] { 3L, 1L, 2L }));
        }
    }
}
