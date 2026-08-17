using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.DTOs.Folder.Order;
using OrderFlow.Application.Helper.Attributes;
using OrderFlow.Application.Helper.Exception;
using OrderFlow.Application.Helper.Exception.Enums;
using OrderFlow.Application.IPatterns;
using OrderFlow.Application.IServices;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Application.Services
{
    [Scoped]
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateOrderResDto> Create(CreateOrderReqDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Optionally verify user exists
                var user = await _unitOfWork.UserRepository.Get(u => u.Id == dto.UserId, tracked: false);
                if (user is null)
                {
                    throw new InternalServerErrorException(
                    "User not found",
                    _CriticalLevel.Three);
                }
                var items = new List<OrderItem>();

                // add items
                if (dto.Items != null && dto.Items.Any())
                {
                    foreach (var item in dto.Items)
                    {
                        Product? product = await _unitOfWork.ProductRepository.Get(p => p.Id == item.ProductId, tracked: true);
                        if (product!.Stock < item.Quantity)
                        {
                            throw new InternalServerErrorException(
                                "Quantity is higher than product`s stock",
                                _CriticalLevel.Three);
                        }
                        var orderItem = OrderItem.Create(item.ProductId, item.Quantity);
                        orderItem.UnitPrice = product.Price;
                        product.DecreaseStock(item.Quantity);
                        items.Add(orderItem);
                    }
                }

                var orderNumber = await _unitOfWork.OrderRepository.GetLastOrderNumber();
                orderNumber++;

                var order = Order.Create(dto.UserId, orderNumber, items, dto.CreatedBy);

                await _unitOfWork.OrderRepository.Add(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new CreateOrderResDto { Id = order.Id };
            }
            catch (DomainValidationException ex)
            {
                throw new BadRequestException(
                    ex.Message,
                    _CriticalLevel.Zero);
            }
            catch (DbUpdateException)
            {
                throw new InternalServerErrorException(
                    "Try again later",
                    _CriticalLevel.Five);
            }
        }

        public async Task<GetByIdOrderResDto> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.Get(
                o => o.Id == id,
                [x => x.OrderItems, x => x.User],
                tracked: false);
                if (order is null)
                {
                    throw new InternalServerErrorException(
                    "Order not found",
                    _CriticalLevel.Three);
                }
                ;

                return new GetByIdOrderResDto
                {
                    UserId = order.UserId,
                    OrderNumber = order.OrderNumber,
                    Status = order.StatusId.ToString(),
                    Total = order.Total,
                    Items = order.OrderItems.Select(oi => new GetByIdOrderResDto_Item
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        Total = oi.Total
                    }).ToList(),
                    CreatedAt = order.CreateDate,
                    ConfirmedAt = order.ConfirmedAt,
                    ModifiedAt = order.ModifiedDate
                };
            }
            catch (DomainValidationException ex)
            {
                throw new BadRequestException(
                    ex.Message,
                    _CriticalLevel.Zero);
            }
            catch (DbUpdateException)
            {
                throw new InternalServerErrorException(
                    "Try again later",
                    _CriticalLevel.Five);
            }
        }

        public async Task<GetAllOrderResDto> List(CancellationToken cancellationToken = default)
        {
            try
            {
                var orders = await _unitOfWork.OrderRepository.GetAll(
                    includes: [x => x.OrderItems, x => x.User],
                    tracked: false);
                return new GetAllOrderResDto
                {
                    Orders = orders.Select(o => new GetAllOrderResDto_Order
                    {
                        Id = o.Id,
                        OrderNumber = o.OrderNumber,
                        Status = o.StatusId.ToString(),
                        Total = o.Total,
                        UserId = o.UserId,
                        CreatedAt = o.CreateDate
                    }).OrderBy(o => o.OrderNumber).ToList(),
                };
            }
            catch (DomainValidationException ex)
            {
                throw new BadRequestException(
                    ex.Message,
                    _CriticalLevel.Zero);
            }
            catch (DbUpdateException)
            {
                throw new InternalServerErrorException(
                    "Try again later",
                    _CriticalLevel.Five);
            }
        }

        public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.Get(
                filter: o => o.Id == id,
                [x => x.OrderItems],
                tracked: true);
                if (order is null)
                {
                    throw new InternalServerErrorException(
                    "Order not found",
                    _CriticalLevel.Three);
                }
                if (order.StatusId == _OrderStatus.Confirmed)
                {
                    throw new InternalServerErrorException(
                    "You can not delete confirmed order",
                    _CriticalLevel.Three);
                }
                foreach (var item in order.OrderItems)
                {
                    var product = await _unitOfWork.ProductRepository.Get(p => p.Id == item.ProductId, tracked: true);
                    product!.IncreaseStock(item.Quantity);
                }

                await _unitOfWork.OrderRepository.Remove(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DomainValidationException ex)
            {
                throw new BadRequestException(
                    ex.Message,
                    _CriticalLevel.Zero);
            }
            catch (DbUpdateException)
            {
                throw new InternalServerErrorException(
                    "Try again later",
                    _CriticalLevel.Five);
            }
        }

        public async Task<bool> Confirm(ConfirmOrderReqDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.Get(filter: o => o.Id == dto.OrderId, tracked: true);
                if (order is null)
                {
                    throw new InternalServerErrorException(
                    "Order not found",
                    _CriticalLevel.Three);
                }

                order.Confirm(dto.ModifiedBy);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DomainValidationException ex)
            {
                throw new BadRequestException(
                    ex.Message,
                    _CriticalLevel.Zero);
            }
            catch (DbUpdateException)
            {
                throw new InternalServerErrorException(
                    "Try again later",
                    _CriticalLevel.Five);
            }
        }

        public async Task<bool> Cancel(CancelOrderReqDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.Get(
                    filter: o => o.Id == dto.OrderId,
                    includes: [x => x.OrderItems],
                    tracked: true);
                if (order is null)
                {
                    throw new InternalServerErrorException(
                    "Order not found",
                    _CriticalLevel.Three);
                }

                order.Cancel(dto.ModifiedBy);
                foreach (var item in order.OrderItems)
                {
                    var product = await _unitOfWork.ProductRepository.Get(p => p.Id == item.ProductId, tracked: true);
                    product!.IncreaseStock(item.Quantity);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DomainValidationException ex)
            {
                throw new BadRequestException(
                    ex.Message,
                    _CriticalLevel.Zero);
            }
            catch (DbUpdateException)
            {
                throw new InternalServerErrorException(
                    "Try again later",
                    _CriticalLevel.Five);
            }
        }
    }
}
