using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Context;
using OrderFlow.Infrastructure.Repositories.IRepositories;

namespace OrderFlow.Infrastructure.Repositories
{
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        private readonly OrderFlowContext _context;
        public OrderItemRepository(OrderFlowContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderItem entity)
        {
            _context.OrderItems.Update(entity);
        }
    }
}
