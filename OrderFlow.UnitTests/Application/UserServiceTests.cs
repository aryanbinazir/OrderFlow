using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using OrderFlow.Application.DTOs.Folder.User;
using OrderFlow.Application.IPatterns;
using OrderFlow.Application.Services;
using OrderFlow.Application.Helper.Exception;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Repositories.IRepositories;
using Xunit;
using OrderFlow.Application.Helper.Attributes;

namespace OrderFlow.UnitTests.Application
{
    public class UserServiceTests
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly UserService svc;
        public UserServiceTests()
        {
            _unitOfWork.UserRepository.Returns(_userRepository);

            svc = new UserService(_unitOfWork);
        }

        [Fact]
        public async Task Create_Should_Return_UserId_When_Request_Is_Valid()
        {
            // Arrange
            var dto = new CreateUserReqDto { Email = "a@b.com", PasswordHash = "pwd", DisplayName = "Name" };
            _userRepository.Any(Arg.Any<Expression<Func<User, bool>>>()).Returns(Task.FromResult(false));
            _userRepository.Add(Arg.Any<User>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // Act
            var res = await svc.Create(dto);
            var user = User.Create(dto.Email, HashUtils.ComputeSha256(dto.PasswordHash), dto.DisplayName, dto.CreatedBy);

            // Assert
            res.Should().NotBeNull();
            res.Id.Should().NotBe(Guid.Empty);
            await _userRepository.Received(1).Add(Arg.Is<User>(u => u.Email == user.Email && u.PasswordHash == user.PasswordHash));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Create_Should_Throw_InternalServerErrorException_When_Email_Already_Exists()
        {
            // Arrange
            var dto = new CreateUserReqDto { Email = "dup@x.com", PasswordHash = "p" };
            _userRepository.Any(Arg.Any<Expression<Func<User, bool>>>()).Returns(Task.FromResult(true));

            // Act
            var act = async () => await svc.Create(dto);

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("A user with the same email is already exist");
            await _userRepository.DidNotReceive().Add(Arg.Any<User>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Create_Should_Throw_BadRequestException_When_DomainValidationFails()
        {
            // Arrange: invalid email triggers DomainValidationException in User.Create
            var dto = new CreateUserReqDto { Email = "", PasswordHash = "pwd" };
            _userRepository.Any(Arg.Any<Expression<Func<User, bool>>>()).Returns(Task.FromResult(false));

            // Act
            var act = async () => await svc.Create(dto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>();
            await _userRepository.DidNotReceive().Add(Arg.Any<User>());
        }

        [Fact]
        public async Task Create_Should_Put_HashPassword_Inside_The_Password()
        {
            // Arrange
            var dto = new CreateUserReqDto { Email = "test@example.com", PasswordHash = "pwd" };
            _userRepository.Any(Arg.Any<Expression<Func<User, bool>>>()).Returns(Task.FromResult(false));
            _userRepository.Add(Arg.Any<User>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            var hashPassword = HashUtils.ComputeSha256(dto.PasswordHash);

            // Act
            var res = await svc.Create(dto);
            var user = User.Create(dto.Email, hashPassword, dto.DisplayName, dto.CreatedBy);

            // Assert
            res.Should().NotBeNull();
            res.Id.Should().NotBe(Guid.Empty);
            await _userRepository.Received(1).Add(Arg.Is<User>(u => u.Email == dto.Email && u.PasswordHash == hashPassword));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Create_Should_Throw_InternalServerErrorException_When_DbUpdateException_Occurs()
        {
            // Arrange
            var dto = new CreateUserReqDto { Email = "x@y.com", PasswordHash = "pwd" };
            _userRepository.Any(Arg.Any<Expression<Func<User, bool>>>()).Returns(Task.FromResult(false));
            _userRepository.When(x => x.Add(Arg.Any<User>())).Do(call => { throw new DbUpdateException(); });

            // Act
            var act = async () => await svc.Create(dto);

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Try again later");
        }

        [Fact]
        public async Task GetById_Should_Return_User_When_User_Exists()
        {
            // Arrange
            var user = User.Create("u@d.com", "hash", "Display", null);
            var item = OrderItem.Create(Guid.NewGuid(), 1);
            var order = Order.Create(user.Id, 123, new List<OrderItem> { item });
            user.Orders.Add(order);

            _userRepository.Get(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult(user));

            // Act
            var dto = await svc.GetById(user.Id);

            // Assert
            dto.Should().NotBeNull();
            dto.Email.Should().Be(user.Email);
            dto.DisplayName.Should().Be(user.DisplayName);
            dto.RoleName.Should().Be(user.RoleId.ToString());
            dto.Orders.Should().HaveCount(1);
            dto.Orders[0].OrderNumber.Should().Be(123);
        }

        [Fact]
        public async Task GetById_Should_Throw_InternalServerErrorException_When_User_Not_Found()
        {
            // Arrange
            _userRepository.Get(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<User?>(null));
        
            // Act
            var act = async () => await svc.GetById(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("User Not found");
        }

        [Fact]
        public async Task List_Should_Return_Users_When_Users_Exist()
        {
            // Arrange
            var u1 = User.Create("a@x.com", "h", "A", null);
            var u2 = User.Create("b@x.com", "h2", "B", null);
            var list = new List<User> { u1, u2 };
            _userRepository.GetAll(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<IEnumerable<User>>(list));

            // Act
            var res = await svc.List();

            // Assert
            res.Users.Should().HaveCount(2);
            res.Users[0].Email.Should().Be(u1.Email);
            res.Users[1].Email.Should().Be(u2.Email);
        }

        [Fact]
        public async Task UpdateDisplayName_Should_Return_UserId_When_User_Exists()
        {
            // Arrange
            var user = User.Create("ud@x.com", "h", "Old", null);
            var dto = new UpdateUserReqDto { DisplayName = "New", ModifiedBy = Guid.NewGuid() };
            _userRepository.Get(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>()).Returns(Task.FromResult(user));
            _userRepository.Update(Arg.Any<User>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // Act
            var res = await svc.UpdateDisplayName(user.Id, dto);

            // Assert
            res.Id.Should().Be(user.Id);
            user.DisplayName.Should().Be("New");
            await _userRepository.Received(1).Update(Arg.Is<User>(u => u.DisplayName == "New"));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateDisplayName_Should_Throw_InternalServerErrorException_When_User_Not_Found()
        {
            // Arrange
            _userRepository.Get(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>()).Returns(Task.FromResult<User?>(null));

            // Act
            var act = async () => await svc.UpdateDisplayName(Guid.NewGuid(), new UpdateUserReqDto());

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("User Not found");
            await _userRepository.DidNotReceive().Update(Arg.Any<User>());
        }

        [Fact]
        public async Task PromoteToAdmin_Should_Return_True_When_User_Exists()
        {
            // Arrange
            var user = User.Create("p@a.com", "h", null, null);
            var dto = new PromoteToAdminReqDto { UserId = user.Id, ModifiedBy = Guid.NewGuid() };
            _userRepository.Get(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>()).Returns(Task.FromResult(user));
            _userRepository.Update(Arg.Any<User>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // Act
            var res = await svc.PromoteToAdmin(dto);

            // Assert
            res.Should().BeTrue();
            user.RoleId.Should().Be(Domain.Enums._UserRole.Admin);
            await _userRepository.Received(1).Update(Arg.Is<User>(u => u.RoleId == Domain.Enums._UserRole.Admin));
        }

        [Fact]
        public async Task PromoteToAdmin_Should_Throw_BadRequestException_When_User_Already_Admin()
        {
            // Arrange
            var user = User.Create("adm@x.com", "h", null, null);
            user.RoleId = Domain.Enums._UserRole.Admin;
            var dto = new PromoteToAdminReqDto { UserId = user.Id };
            _userRepository.Get(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>()).Returns(Task.FromResult(user));

            // Act
            var act = async () => await svc.PromoteToAdmin(dto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>().WithMessage("User is already an admin.");
            await _userRepository.DidNotReceive().Update(Arg.Any<User>());
        }

        [Fact]
        public async Task Delete_Should_Return_True_When_User_Exists()
        {
            // Arrange
            var user = User.Create("del@x.com", "h", null, null);
            _userRepository.Get(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult(user));
            _userRepository.Remove(Arg.Any<User>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // Act
            var res = await svc.Delete(user.Id);

            // Assert
            res.Should().BeTrue();
            await _userRepository.Received(1).Remove(Arg.Is<User>(u => u.Id == user.Id));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Delete_Should_Throw_InternalServerErrorException_When_User_Not_Found()
        {
            // Arrange
            _userRepository.Get(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<User?>(null));

            // Act
            var act = async () => await svc.Delete(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("User Not found");
            await _userRepository.DidNotReceive().Remove(Arg.Any<User>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
