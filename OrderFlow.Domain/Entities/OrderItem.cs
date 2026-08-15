using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Domain.Entities
{
    public class OrderItem : BaseEntity<Guid>
    {
        public decimal UnitPrice { get; set; }
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

        public static OrderItem Create(
            Guid productId,
            int quantity,
            Guid? createdBy = null)
        {
            if (productId == Guid.Empty) throw new DomainValidationException("Invalid product id.");
            if (quantity <= 0) throw new DomainValidationException("Quantity must be greater than zero.");

            var orderItem = new OrderItem
            {
                ProductId = productId,
                Quantity = quantity
            };
            orderItem.CreateRecord(createdBy);

            return orderItem;
        }
    }
}
