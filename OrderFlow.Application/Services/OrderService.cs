using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OrderFlow.Application.DTOs.Folder.Order;
using OrderFlow.Application.IPatterns;
using OrderFlow.Application.IServices;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateOrderResDto> Create(CreateOrderReqDto dto, CancellationToken cancellationToken = default)
        {
            // Optionally verify user exists
            var user = await _unitOfWork.UserRepository.Get(u => u.Id == dto.UserId, tracked: false);
            if (user is null) throw new DomainValidationException("User not found.");
            var items = new List<OrderItem>();
            var orderNumber = await _unitOfWork.OrderRepository.GetNextOrderNumber();
            orderNumber++; 

            var order = Order.Create(dto.UserId, orderNumber, items, dto.CreatedBy);

            // add items
            if (dto.Items != null && dto.Items.Any())
            {
                foreach (var item in dto.Items)
                {
                    order.AddItem(item.ProductId, item.UnitPrice, item.Quantity);
                }
            }

            await _unitOfWork.OrderRepository.Add(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateOrderResDto { Id = order.Id };
        }

        public async Task<GetByIdOrderResDto> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await _unitOfWork.OrderRepository.Get(o => o.Id == id, includeProperties: "Orders", tracked: false);
            if (order is null) throw new DomainValidationException("Order not found.");

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
            };
        }

        public async Task<GetAllOrderResDto> GetAll(CancellationToken cancellationToken = default)
        {
            var orders = await _unitOfWork.OrderRepository.GetAll(includeProperties: "Orders", tracked: false);
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

        public async Task Delete(DeleteOrderReqDto dto, CancellationToken cancellationToken = default)
        {
            var order = await _unitOfWork.OrderRepository.Get(o => o.Id == dto.OrderId, tracked: true);
            if (order is null) throw new DomainValidationException("Order not found.");

            order.SoftDelete(dto.ModifiedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task Confirm(ConfirmOrderReqDto dto, CancellationToken cancellationToken = default)
        {
            var order = await _unitOfWork.OrderRepository.Get(o => o.Id == dto.OrderId, tracked: true);
            if (order is null) throw new DomainValidationException("Order not found.");

            order.Confirm(dto.ModifiedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task Cansel(CancelOrderReqDto dto, CancellationToken cancellationToken = default)
        {
            var order = await _unitOfWork.OrderRepository.Get(o => o.Id == dto.OrderId, tracked: true);
            if (order is null) throw new DomainValidationException("Order not found.");

            order.Cancel(dto.ModifiedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
