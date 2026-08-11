using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Domain.Entities
{
    public class OrderItem : BaseEntity<Guid>
    {
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal Total
        {
            get => UnitPrice * Quantity;
            private set { }
        }
        public Guid ProductId { get; private set; }
        public Product Product { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public OrderItem Create(Guid productId, Guid OrderId, decimal unitPrice, int quantity, Guid? createdBy = null)
        {
            if (productId == Guid.Empty) throw new DomainValidationException("Invalid product id.");
            if (unitPrice < 0) throw new DomainValidationException("Unit price must be non-negative.");
            if (quantity <= 0) throw new DomainValidationException("Quantity must be greater than zero.");

            var orderItem = new OrderItem
            {
                ProductId = productId,
                OrderId = OrderId,
                UnitPrice = unitPrice,
                Quantity = quantity
            };
            orderItem.CreateRecord(createdBy);

            return orderItem;

        }
    }
}
