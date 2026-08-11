using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities
{
    public class OrderStatus : BaseLookupEntity<_OrderStatus>
    {
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
