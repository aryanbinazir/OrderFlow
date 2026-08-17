using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using OrderFlow.Application.DTOs.Folder.Order;
using OrderFlow.Application.IPatterns;
using OrderFlow.Application.Services;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Infrastructure.Repositories.IRepositories;
using OrderFlow.Application.Helper.Exception;
using System.Linq.Expressions;

namespace OrderFlow.UnitTests.Application
{
    public class OrderServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _orderRepository = Substitute.For<IOrderRepository>();
            _productRepository = Substitute.For<IProductRepository>();
            _userRepository = Substitute.For<IUserRepository>();

            _unitOfWork = Substitute.For<IUnitOfWork>();
            _unitOfWork.OrderRepository.Returns(_orderRepository);
            _unitOfWork.ProductRepository.Returns(_productRepository);
            _unitOfWork.UserRepository.Returns(_userRepository);

            _service = new OrderService(_unitOfWork);
        }

        // ---------------- CREATE ----------------

        [Fact]
        public async Task Create_Should_Return_OrderId_When_Request_Is_Valid()
        {
            // Arrange
            var user = User.Create("u@example.com", "hash");
            var product = Product.Create("P1", 10m, null, stock: 5);
            var dto = new CreateOrderReqDto
            {
                UserId = user.Id,
                Items = new List<CreateOrderReqDto_Item>
                {
                    new() { ProductId = product.Id, Quantity = 2 }
                },
                CreatedBy = Guid.NewGuid()
            };

            // repository setups
            _userRepository.Get(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<User?>(user));

            // product repo should return the actual product (tracked true)
            _productRepository.Get(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Product?>(product));

            _orderRepository.GetLastOrderNumber().Returns(100);

            Order? captured = null;
            _orderRepository.When(r => r.Add(Arg.Any<Order>())).Do(ci => captured = ci.Arg<Order>());

            // Act
            var result = await _service.Create(dto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            captured.Should().NotBeNull();
            result.Id.Should().Be(captured!.Id);

            // Order number incremented
            captured.OrderNumber.Should().Be(101);

            // Order has expected item with correct unit price and quantity
            captured.OrderItems.Should().HaveCount(1);
            var item = captured.OrderItems.First();
            item.Quantity.Should().Be(2);
            item.UnitPrice.Should().Be(product.Price);
            item.Total.Should().Be(product.Price * item.Quantity);

            // Product stock decreased
            product.Stock.Should().Be(3);

            await _orderRepository.Received(1).Add(Arg.Is<Order>(o => o.Id == captured.Id));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Create_Should_Throw_InternalServerErrorException_When_User_Does_Not_Exist()
        {
            // Arrange
            var dto = new CreateOrderReqDto { UserId = Guid.NewGuid() };
            _userRepository.Get(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<User?>(null));

            // Act
            Func<Task> act = async () => await _service.Create(dto);

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("User not found");
            await _orderRepository.DidNotReceive().Add(Arg.Any<Order>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Create_Should_Throw_InternalServerErrorException_When_Product_Stock_Is_Insufficient()
        {
            // Arrange
            var user = User.Create("a@b.com", "hash");
            var product = Product.Create("P1", 5m, null, stock: 1);
            var dto = new CreateOrderReqDto
            {
                UserId = user.Id,
                Items = new List<CreateOrderReqDto_Item> { new() { ProductId = product.Id, Quantity = 2 } }
            };

            _userRepository.Get(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<User?>(user));
            _productRepository.Get(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Product?>(product));

            // Act
            Func<Task> act = async () => await _service.Create(dto);

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Quantity is higher than product`s stock");
            await _orderRepository.DidNotReceive().Add(Arg.Any<Order>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Create_Should_Throw_BadRequestException_When_OrderItem_Is_Empty()
        {
            // Arrange
            var user = User.Create("x@y.com", "hash");
            var dto = new CreateOrderReqDto { UserId = user.Id, Items = new List<CreateOrderReqDto_Item>() };

            _userRepository.Get(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<User?>(user));

            // Act
            Func<Task> act = async () => await _service.Create(dto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>().WithMessage("Order must contain at least one item.");
            await _orderRepository.DidNotReceive().Add(Arg.Any<Order>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Create_Should_Throw_InternalServerErrorException_When_SaveChanges_Throws_DbUpdateException()
        {
            // Arrange
            var user = User.Create("u2@example.com", "hash");
            var product = Product.Create("P2", 20m, null, stock: 5);
            var dto = new CreateOrderReqDto
            {
                UserId = user.Id,
                Items = new List<CreateOrderReqDto_Item> { new() { ProductId = product.Id, Quantity = 1 } }
            };

            _userRepository.Get(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<Expression<Func<User, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<User?>(user));
            _productRepository.Get(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Product?>(product));
            _orderRepository.GetLastOrderNumber().Returns(0);

            _unitOfWork.When(u => u.SaveChangesAsync(Arg.Any<CancellationToken>())).Do(ci => throw new DbUpdateException());

            // Act
            Func<Task> act = async () => await _service.Create(dto);

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Try again later");
        }

        // ---------------- GET BY ID ----------------

        [Fact]
        public async Task GetById_Should_Return_Order_When_Order_Exists()
        {
            // Arrange
            var product = Product.Create("pq", 7.5m, null, 10);
            var orderItem = OrderItem.Create(product.Id, 3);
            orderItem.UnitPrice = product.Price;
            var items = new List<OrderItem> { orderItem };
            var order = Order.Create(Guid.NewGuid(), 10, items, Guid.NewGuid());

            _orderRepository.Get(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<Expression<Func<Order, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Order?>(order));

            // Act
            var result = await _service.GetById(order.Id);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(order.UserId);
            result.OrderNumber.Should().Be(order.OrderNumber);
            result.Status.Should().Be(order.StatusId.ToString());
            result.Total.Should().Be(order.Total);
            result.Items.Should().HaveCount(1);
            var mapped = result.Items.First();
            mapped.ProductId.Should().Be(product.Id);
            mapped.Quantity.Should().Be(3);
            mapped.UnitPrice.Should().Be(7.5m);
            mapped.Total.Should().Be(7.5m * 3);
        }

        [Fact]
        public async Task GetById_Should_Throw_InternalServerErrorException_When_Order_Does_Not_Exist()
        {
            // Arrange
            _orderRepository.Get(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<Expression<Func<Order, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Order?>(null));

            // Act
            Func<Task> act = async () => await _service.GetById(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Order not found");
        }

        // ---------------- LIST ----------------

        [Fact]
        public async Task List_Should_Return_Orders_Ordered_By_OrderNumber()
        {
            // Arrange
            var o1 = Order.Create(Guid.NewGuid(), 2, new List<OrderItem> { OrderItem.Create(Guid.NewGuid(), 1) }, Guid.NewGuid());
            var o2 = Order.Create(Guid.NewGuid(), 1, new List<OrderItem> { OrderItem.Create(Guid.NewGuid(), 2) }, Guid.NewGuid());
            var orders = new List<Order> { o1, o2 };
            _orderRepository.GetAll(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<Expression<Func<Order, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<IEnumerable<Order>>(orders));

            // Act
            var result = await _service.List();

            // Assert
            result.Should().NotBeNull();
            result.Orders.Should().HaveCount(2);
            result.Orders.Should().BeInAscendingOrder(o => o.OrderNumber);
            result.Orders.Select(o => o.OrderNumber).Should().Equal(1, 2);
        }

        [Fact]
        public async Task List_Should_Return_Empty_When_No_Orders()
        {
            // Arrange
            _orderRepository.GetAll(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<Expression<Func<Order, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<IEnumerable<Order>>(new List<Order>()));

            // Act
            var result = await _service.List();

            // Assert
            result.Should().NotBeNull();
            result.Orders.Should().BeEmpty();
        }

        // ---------------- DELETE ----------------

        [Fact]
        public async Task Delete_Should_Return_True_And_Restore_Product_Stock_When_Order_Is_Deleted()
        {
            // Arrange
            var product1 = Product.Create("P1", 5m, null, stock: 10);
            var product2 = Product.Create("P2", 8m, null, stock: 20);
            var product3 = Product.Create("P3", 12m, null, stock: 30);

            var item1 = OrderItem.Create(product1.Id, 5);
            var item2 = OrderItem.Create(product2.Id, 7);
            var item3 = OrderItem.Create(product3.Id, 10);

            var order = Order.Create(
                Guid.NewGuid(),
                1,
                new List<OrderItem> { item1, item2, item3 },
                Guid.NewGuid());

            _orderRepository.Get(
                Arg.Any<Expression<Func<Order, bool>>>(),
                Arg.Any<Expression<Func<Order, object>>[]>(),
                Arg.Any<bool>())
                .Returns(Task.FromResult<Order?>(order));

            _productRepository.Get(
                Arg.Any<Expression<Func<Product, bool>>>(),
                Arg.Any<Expression<Func<Product, object>>[]>(),
                Arg.Any<bool>())
                .Returns(callInfo =>
                {
                    var expression =
                        callInfo.ArgAt<Expression<Func<Product, bool>>>(0);

                    if (expression.Compile()(product1))
                        return Task.FromResult<Product?>(product1);

                    if (expression.Compile()(product2))
                        return Task.FromResult<Product?>(product2);

                    if (expression.Compile()(product3))
                        return Task.FromResult<Product?>(product3);

                    return Task.FromResult<Product?>(null);
                });

            // Act
            var result = await _service.Delete(order.Id);

            // Assert
            result.Should().BeTrue();

            product1.Stock.Should().Be(15); // 10 + 5
            product2.Stock.Should().Be(27); // 20 + 7
            product3.Stock.Should().Be(40); // 30 + 10

            await _orderRepository
                .Received(1)
                .Remove(Arg.Is<Order>(o => o.Id == order.Id));

            await _unitOfWork
                .Received(1)
                .SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Delete_Should_Throw_InternalServerErrorException_When_Order_Is_Confirmed()
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid(), 1, new List<OrderItem> { OrderItem.Create(Guid.NewGuid(), 1) }, Guid.NewGuid());
            // set status to Confirmed
            order.Confirm();

            _orderRepository.Get(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<Expression<Func<Order, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Order?>(order));

            // Act
            Func<Task> act = async () => await _service.Delete(order.Id);

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("You can not delete confirmed order");
            await _orderRepository.DidNotReceive().Remove(Arg.Any<Order>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Delete_Should_Throw_InternalServerErrorException_When_Order_Does_Not_Exist()
        {
            // Arrange
            _orderRepository.Get(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<Expression<Func<Order, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Order?>(null));

            // Act
            Func<Task> act = async () => await _service.Delete(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Order not found");
            await _orderRepository.DidNotReceive().Remove(Arg.Any<Order>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        // ---------------- CONFIRM ----------------

        [Fact]
        public async Task Confirm_Should_Return_True_When_Order_Is_Confirmed()
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid(), 1, new List<OrderItem> { OrderItem.Create(Guid.NewGuid(), 1) }, Guid.NewGuid());
            _orderRepository.Get(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<Expression<Func<Order, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Order?>(order));

            var dto = new ConfirmOrderReqDto { OrderId = order.Id, ModifiedBy = Guid.NewGuid() };

            // Act
            var result = await _service.Confirm(dto);

            // Assert
            result.Should().BeTrue();
            order.StatusId.Should().Be(_OrderStatus.Confirmed);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Confirm_Should_Throw_InternalServerErrorException_When_Order_Does_Not_Exist()
        {
            // Arrange
            _orderRepository.Get(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<Expression<Func<Order, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Order?>(null));
            var dto = new ConfirmOrderReqDto { OrderId = Guid.NewGuid() };

            // Act
            Func<Task> act = async () => await _service.Confirm(dto);

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Order not found");
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        // ---------------- CANCEL ----------------

        [Fact]
        public async Task Cancel_Should_Return_True_And_Restore_Product_Stock_When_Order_Is_Cancelled()
        {
            // Arrange
            var product1 = Product.Create("P1", 3m, null, stock: 2);
            var product2 = Product.Create("P2", 5m, null, stock: 10);
            var product3 = Product.Create("P3", 7m, null, stock: 20);

            var item1 = OrderItem.Create(product1.Id, 2);
            var item2 = OrderItem.Create(product2.Id, 5);
            var item3 = OrderItem.Create(product3.Id, 10);

            var order = Order.Create(
                Guid.NewGuid(),
                5,
                new List<OrderItem> { item1, item2, item3 },
                Guid.NewGuid());

            _orderRepository.Get(
                Arg.Any<Expression<Func<Order, bool>>>(),
                Arg.Any<Expression<Func<Order, object>>[]>(),
                Arg.Any<bool>())
                .Returns(Task.FromResult<Order?>(order));

            _productRepository.Get(
                Arg.Any<Expression<Func<Product, bool>>>(),
                Arg.Any<Expression<Func<Product, object>>[]>(),
                Arg.Any<bool>())
                .Returns(callInfo =>
                {
                    var expression = callInfo.ArgAt<Expression<Func<Product, bool>>>(0);

                    if (expression.Compile()(product1))
                        return Task.FromResult<Product?>(product1);

                    if (expression.Compile()(product2))
                        return Task.FromResult<Product?>(product2);

                    if (expression.Compile()(product3))
                        return Task.FromResult<Product?>(product3);

                    return Task.FromResult<Product?>(null);
                });

            var dto = new CancelOrderReqDto
            {
                OrderId = order.Id,
                ModifiedBy = Guid.NewGuid()
            };

            // Act
            var result = await _service.Cancel(dto);

            // Assert
            result.Should().BeTrue();

            product1.Stock.Should().Be(4);   // 2 + 2
            product2.Stock.Should().Be(15);  // 10 + 5
            product3.Stock.Should().Be(30);  // 20 + 10

            await _unitOfWork
                .Received(1)
                .SaveChangesAsync(Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Cancel_Should_Throw_InternalServerErrorException_When_Order_Does_Not_Exist()
        {
            // Arrange
            _orderRepository.Get(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<Expression<Func<Order, object>>[]>(), Arg.Any<bool>())
                .Returns(Task.FromResult<Order?>(null));
            var dto = new CancelOrderReqDto { OrderId = Guid.NewGuid() };

            // Act
            Func<Task> act = async () => await _service.Cancel(dto);

            // Assert
            await act.Should().ThrowAsync<InternalServerErrorException>().WithMessage("Order not found");
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
