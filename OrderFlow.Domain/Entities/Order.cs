using OrderFlow.Domain.Enums;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Domain.Entities
{
    public class Order : BaseEntity<Guid>
    {
       
        public int OrderNumber { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }

        public _OrderStatus StatusId { get; set; } = _OrderStatus.Draft;
        public OrderStatus OrderStatus { get; private set; }

        private decimal _total;
        public decimal Total
        {
            get => OrderItems.Sum(i => i.Total);
            private set => _total = value;
        }

        public ICollection<OrderItem> OrderItems = new List<OrderItem>();
        public Guid UserId { get; private set; }
        public User User { get; private set; }

        public static Order Create(Guid userId, int orderNumber, ICollection<OrderItem> OrderItems, Guid? createdBy = null)
        {
            if (userId == Guid.Empty) throw new DomainValidationException("Invalid user id.");
            if (orderNumber <= 0) throw new DomainValidationException("Order number must be a positive integer.");
            if (OrderItems == null || !OrderItems.Any()) throw new DomainValidationException("Order must contain at least one item.");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderNumber = orderNumber,
                StatusId = _OrderStatus.Draft,
                OrderItems = OrderItems
            };
            order.CreateRecord(createdBy);

            return order;
        }

        public void AddItem(Guid productId, decimal unitPrice, int quantity, Guid? modifiedBy = null)
        {
            EnsureMutable();
            var orderItem = new OrderItem();
            var newOrderItem = orderItem.Create(productId, Id, unitPrice, quantity);
            OrderItems.Add(newOrderItem);
            TouchRecord(modifiedBy);
        }

        public void RemoveItem(Guid orderItemId, Guid? modifiedBy = null)
        {
            EnsureMutable();
            var item = OrderItems.FirstOrDefault(i => i.Id == orderItemId);

            if (item is null)
                throw new DomainValidationException("Order item not found.");

            OrderItems.Remove(item);
        }

        public void Confirm(Guid? modifiedBy = null)
        {
            if (StatusId != _OrderStatus.Draft) throw new DomainValidationException("Only draft orders can be confirmed.");
            if (!OrderItems.Any()) throw new DomainValidationException("Cannot confirm an empty order.");
            StatusId = _OrderStatus.Confirmed;
            ConfirmedAt = DateTime.Now;
            TouchRecord(modifiedBy);
        }

        public void Cancel(Guid? modifiedBy = null)
        {
            if (StatusId == _OrderStatus.Cancelled) return;
            StatusId = _OrderStatus.Cancelled;
            TouchRecord(modifiedBy);
        }

        private void EnsureMutable()
        {
            if (StatusId != _OrderStatus.Draft) throw new DomainValidationException("Order cannot be modified in its current state.");
        }
    }
}
