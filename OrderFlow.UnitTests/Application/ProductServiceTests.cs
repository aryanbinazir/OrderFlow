using FluentAssertions;
using NSubstitute;
using OrderFlow.Application.DTOs.Folder.Product;
using OrderFlow.Application.Helper.Exception;
using OrderFlow.Application.IPatterns;
using OrderFlow.Application.Services;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Repositories.IRepositories;
using System.Linq.Expressions;

namespace OrderFlow.UnitTests.Application
{
    public class ProductServiceTests
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
        private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
        private readonly ProductService svc;

        public ProductServiceTests()
        {
            _unitOfWork.ProductRepository.Returns(_productRepository);
            _unitOfWork.CategoryRepository.Returns(_categoryRepository);

            svc = new ProductService(_unitOfWork);
        }

        [Fact]
        public async Task Create_Should_Add_Product_And_Return_Id_When_NoCategoryOr_CategoryExists()
        {
            // Arrange
            var dto = new CreateProductReqDto { Name = "P1", Price = 5.0m, Stock = 10 };
            _productRepository.Add(Arg.Any<Product>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // Act
            var res = await svc.Create(dto);

            // Assert
            res.Should().NotBeNull();
            res.Id.Should().NotBe(Guid.Empty);
            await _productRepository.Received(1).Add(Arg.Is<Product>(p => p.Name == dto.Name && p.Price == dto.Price && p.Stock == dto.Stock));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Create_Should_Throw_InternalServerError_When_Category_NotFound()
        {
            // Arrange
            var dto = new CreateProductReqDto { Name = "P1", Price = 1m, CategoryId = Guid.NewGuid(), Stock = 1 };
            _categoryRepository.Get(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<Expression<Func<Category, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Category?>(null));

            // Act
            var act = async () => await svc.Create(dto);

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Category not found");
            await _productRepository.DidNotReceive().Add(Arg.Any<Product>());
        }

        [Fact]
        public async Task GetById_Should_Return_Mapped_Dto_When_Product_Exists()
        {
            // Arrange
            var product = Product.Create("Prod", 2.5m, null, 3);
            _productRepository.Get(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult(product));

            // Act
            var dto = await svc.GetById(product.Id);

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(product.Id);
            dto.Name.Should().Be(product.Name);
            dto.Price.Should().Be(product.Price);
            dto.Stock.Should().Be(product.Stock);
            dto.CategoryId.Should().Be(null);
            dto.CategoryId.Should().Be(product.CreateById);
            product.Category.Should().BeNull();
        }

        [Fact]
        public async Task GetById_Should_Throw_InternalServerError_When_NotFound()
        {
            // Arrange
            _productRepository.Get(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Product?>(null));

            // Act
            var act = async () => await svc.GetById(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Product not found");
        }

        [Fact]
        public async Task List_Should_Return_All_Products_Mapped()
        {
            // Arrange
            var p1 = Product.Create("A", 1m, null, 1);
            var p2 = Product.Create("B", 2m, null, 2);
            var list = new List<Product> { p1, p2 };
            _productRepository.GetAll(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<IEnumerable<Product>>(list));

            // Act
            var res = await svc.List();

            // Assert
            res.Should().NotBeNull();
            res.Products.Should().HaveCount(2);
            res.Products.Select(x => x.Name).Should().BeEquivalentTo(new[] { "A", "B" });
        }

        [Fact]
        public async Task Update_Should_Update_And_Return_Id_When_Valid()
        {
            // Arrange
            var product = Product.Create("Old", 1m, null, 5);
            var dto = new UpdateProductReqDto { Name = "New", Price = 2m };
            _productRepository.Get(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult(product));
            _productRepository.Update(Arg.Any<Product>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // Act
            var res = await svc.Update(product.Id, dto);

            // Assert
            res.Should().NotBeNull();
            res.Id.Should().Be(product.Id);
            await _productRepository.Received(1).Update(Arg.Is<Product>(p => p.Name == dto.Name && p.Price == dto.Price));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Update_Should_Throw_InternalServerError_When_NotFound()
        {
            // Arrange
            _productRepository.Get(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Product?>(null));
  
            // Act
            var act = async () => await svc.Update(Guid.NewGuid(), new UpdateProductReqDto());

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Product not found");
            await _productRepository.DidNotReceive().Update(Arg.Any<Product>());
        }

        [Fact]
        public async Task IncreaseProductStock_Should_Increase_And_Return_True_When_Found()
        {
            // Arrange
            var product = Product.Create("X", 1m, null, 1);
            var dto = new IncreaseProductStockReqDto { ProductId = product.Id, Amount = 5 };
            _productRepository.Get(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult(product));
            _productRepository.Update(Arg.Any<Product>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // Act
            var res = await svc.IncreaseProductStock(dto);

            // Assert
            res.Should().BeTrue();
            product.Stock.Should().Be(6);
            await _productRepository.Received(1).Update(Arg.Is<Product>(p => p.Id == product.Id));
        }

        [Fact]
        public async Task IncreaseProductStock_Should_Throw_InternalServerError_When_NotFound()
        {
            // Arrange
            _productRepository.Get(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>()).Returns(Task.FromResult<Product?>(null));
  
            var dto = new IncreaseProductStockReqDto { ProductId = Guid.NewGuid(), Amount = 1 };

            // Act
            var act = async () => await svc.IncreaseProductStock(dto);

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Product not found");
        }

        [Fact]
        public async Task DecreaseProductStock_Should_Decrease_When_EnoughStock()
        {
            // Arrange
            var product = Product.Create("Y", 1m, null, 10);
            var dto = new DecreaseProductStockReqDto { ProductId = product.Id, Amount = 4 };
            _productRepository.Get(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult(product));
            _productRepository.Update(Arg.Any<Product>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // Act
            var res = await svc.DecreaseProductStock(dto);

            // Assert
            res.Should().BeTrue();
            product.Stock.Should().Be(6);
            await _productRepository.Received(1).Update(Arg.Is<Product>(p => p.Id == product.Id));
        }

        [Fact]
        public async Task DecreaseProductStock_Should_Throw_BadRequest_When_InsufficientStock()
        {
            // Arrange
            var product = Product.Create("Z", 1m, null, 2);
            var dto = new DecreaseProductStockReqDto { ProductId = product.Id, Amount = 5 };
            _productRepository.Get(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult(product));

            // Act
            var act = async () => await svc.DecreaseProductStock(dto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>().WithMessage("Insufficient stock.");
            await _productRepository.DidNotReceive().Update(Arg.Any<Product>());
        }

        [Fact]
        public async Task Delete_Should_Remove_And_Return_True_When_Found()
        {
            // Arrange
            var product = Product.Create("D", 1m, null, 1);
            _productRepository.Get(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult(product));
            _productRepository.Remove(Arg.Any<Product>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // Act
            var res = await svc.Delete(product.Id);

            // Assert
            res.Should().BeTrue();
            await _productRepository.Received(1).Remove(Arg.Is<Product>(p => p.Id == product.Id));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Delete_Should_Throw_InternalServerError_When_NotFound()
        {
            // Arrange
            _productRepository.Get(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Product?>(null));

            // Act
            var act = async () => await svc.Delete(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Product not found");
            await _productRepository.DidNotReceive().Remove(Arg.Any<Product>());
        }
    }
}
