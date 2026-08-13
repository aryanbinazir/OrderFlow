using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Context;
using OrderFlow.Infrastructure.Repositories.IRepositories;

namespace OrderFlow.Infrastructure.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly OrderFlowContext _context;
        public OrderRepository(OrderFlowContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Order entity)
        {
            _context.Orders.Update(entity);
        }
    }
}
