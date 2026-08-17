using FluentAssertions;
using NSubstitute;
using OrderFlow.Application.DTOs.Folder.Category;
using OrderFlow.Application.Helper.Exception;
using OrderFlow.Application.IPatterns;
using OrderFlow.Application.Services;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Repositories.IRepositories;
using System.Linq.Expressions;

namespace OrderFlow.UnitTests.Application
{
    public class CategoryServiceTests
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
        private readonly CategoryService svc;

        public CategoryServiceTests()
        {
            _unitOfWork.CategoryRepository.Returns(_categoryRepository);
            
            svc = new CategoryService(_unitOfWork);
        }

        [Fact]
        public async Task Create_Should_Add_Category_And_Return_Id_When_Valid()
        {
            // Arrange
            var dto = new CreateCategoryReqDto { Name = "NewCat", Description = "desc", CreatedBy = Guid.NewGuid() };
            _categoryRepository.Any(Arg.Any<Expression<Func<Category, bool>>>()).Returns(Task.FromResult(false));
            _categoryRepository.Add(Arg.Any<Category>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);


            // Act
            var result = await svc.Create(dto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);
            await _categoryRepository.Received(1).Add(Arg.Is<Category>(c => c.Name == dto.Name));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Create_Should_Throw_InternalServerError_When_Duplicate_Name()
        {
            // Arrange
            var dto = new CreateCategoryReqDto { Name = "Existing" };
            _categoryRepository.Any(Arg.Any<Expression<Func<Category, bool>>>()).Returns(Task.FromResult(true));

            // Act
            var act = async () => await svc.Create(dto);

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>();
            await _categoryRepository.DidNotReceive().Add(Arg.Any<Category>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Create_WithParent_Should_AddChild_To_Parent()
        {
            // Arrange
            var parent = Category.Create("Parent", null, null, null);
            var dto = new CreateCategoryReqDto { Name = "Child", ParentId = parent.Id };

            _categoryRepository.Any(Arg.Any<Expression<Func<Category, bool>>>()).Returns(Task.FromResult(false));
            _categoryRepository.Get(filter: Arg.Any<Expression<Func<Category, bool>>>(), includes: Arg.Any<Expression<Func<Category, object>>[]>(), Arg.Any<bool>())
                .Returns(callInfo => Task.FromResult(parent));
            _categoryRepository.Add(Arg.Any<Category>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);


            // Act
            var result = await svc.Create(dto);

            // Assert
            result.Id.Should().NotBe(Guid.Empty);
            parent.Children.Should().ContainSingle().Which.Name.Should().Be(dto.Name);
            await _categoryRepository.Received(1).Add(Arg.Any<Category>());
        }

        [Fact]
        public async Task GetById_Should_Return_Mapped_Dto_When_Category_Exists()
        {
            // Arrange
            var category = Category.Create("Cat", "desc", null, null);
            var product = Product.Create("P1", 10m, null, 5);
            category.Products.Add(product);
            var child = Category.Create("Child", null, null, null);
            category.Children.Add(child);

            _categoryRepository.Get(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<Expression<Func<Category, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult(category));


            // Act
            var dto = await svc.GetById(category.Id);

            // Assert
            dto.Should().NotBeNull();
            dto.Name.Should().Be(category.Name);
            dto.Products.Should().HaveCount(1);
            category.Children.Should().ContainSingle().Which.Name.Should().Be(child.Name);
            dto.Children.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetById_Should_Throw_InternalServerError_When_NotFound()
        {
            // Arrange
            _categoryRepository.Get(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<Expression<Func<Category, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Category?>(null));

            // Act
            var act = async () => await svc.GetById(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Category not found");
        }

        [Fact]
        public async Task List_Should_Return_All_Categories_Mapped()
        {
            // Arrange
            var c1 = Category.Create("C1", "des2", null, null);
            var c2 = Category.Create("C2", null, null, null);
            var categories = new List<Category> { c1, c2 };

            _categoryRepository.GetAll(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<Expression<Func<Category, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<IEnumerable<Category>>(categories));


            // Act
            var result = await svc.List();

            // Assert
            result.Should().NotBeNull();
            result.Categories.Should().HaveCount(2);
            result.Categories.Select(x => x.Name).Should().BeEquivalentTo(new[] { "C1", "C2" });
            result.Categories.Select(x => x.Description).Should().BeEquivalentTo(new[] { null, "des2" });
        }

        [Fact]
        public async Task Update_Should_Update_And_Return_Id_When_Valid()
        {
            // Arrange
            var category = Category.Create("Old", null, null, null);
            var dto = new UpdateCategoryReqDto { Name = "NewName", Description = "d" };

            _categoryRepository.Get(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<Expression<Func<Category, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult(category));
            _categoryRepository.Any(Arg.Any<Expression<Func<Category, bool>>>()).Returns(Task.FromResult(false));
            _categoryRepository.Update(Arg.Any<Category>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // Act
            var res = await svc.Update(category.Id, dto);

            // Assert
            res.Should().NotBeNull();
            res.Id.Should().Be(category.Id);
            await _categoryRepository.Received(1).Update(Arg.Is<Category>(c => c.Name == dto.Name));
            await _categoryRepository.Received(1).Update(Arg.Is<Category>(c => c.Description == dto.Description));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Update_Should_Throw_InternalServerError_When_NotFound()
        {
            // Arrange
            _categoryRepository.Get(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<Expression<Func<Category, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Category?>(null));
             

            // Act
            var act = async () => await svc.Update(Guid.NewGuid(), new UpdateCategoryReqDto());

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Category not found");
            await _categoryRepository.DidNotReceive().Update(Arg.Any<Category>());
        }

        [Fact]
        public async Task Update_Should_Throw_BadRequest_When_Setting_Parent_To_Self()
        {
            // Arrange
            var category = Category.Create("X", null, null, null);
            var dto = new UpdateCategoryReqDto { ParentId = category.Id };
            _categoryRepository.Get(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<Expression<Func<Category, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult(category));
            _categoryRepository.Any(Arg.Any<Expression<Func<Category, bool>>>()).Returns(Task.FromResult(false));

            // Act
            var act = async () => await svc.Update(category.Id, dto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>().WithMessage("A category cannot be its own parent.");
            await _categoryRepository.DidNotReceive().Update(Arg.Any<Category>());
        }

        [Fact]
        public async Task Update_Should_Throw_InternalServerError_When_Duplicate_Name()
        {
            // Arrange
            var category = Category.Create("duplicate", null, null, null);
            var dto = new UpdateCategoryReqDto { Name = "duplicate" };
            _categoryRepository.Get(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<Expression<Func<Category, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult(category));
            _categoryRepository.Any(Arg.Any<Expression<Func<Category, bool>>>()).Returns(Task.FromResult(true));

            // Act
            var act = async () => await svc.Update(category.Id, dto);

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("A category with the same name already exists.");
            await _categoryRepository.DidNotReceive().Update(Arg.Any<Category>());
        }

        [Fact]
        public async Task Delete_Should_Remove_And_Return_True_When_Found()
        {
            // Arrange
            var category = Category.Create("ToDel", null, null, null);
            _categoryRepository.Get(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<Expression<Func<Category, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult(category));
            _categoryRepository.Remove(Arg.Any<Category>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // Act
            var res = await svc.Delete(category.Id);

            // Assert
            res.Should().BeTrue();
            await _categoryRepository.Received(1).Remove(Arg.Is<Category>(c => c.Id == category.Id));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Delete_Should_Throw_InternalServerError_When_NotFound()
        {
            // Arrange
            _categoryRepository.Get(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<Expression<Func<Category, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Category?>(null));

            // Act
            var act = async () => await svc.Delete(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Category not found");
            await _categoryRepository.DidNotReceive().Remove(Arg.Any<Category>());
        }
    }
}
