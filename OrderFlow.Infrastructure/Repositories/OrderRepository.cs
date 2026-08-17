using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Helper.Attributes;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Context;
using OrderFlow.Infrastructure.Repositories.IRepositories;

namespace OrderFlow.Infrastructure.Repositories
{
    [Scoped]
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly OrderFlowContext _context;
        public OrderRepository(OrderFlowContext context) : base(context)
        {
            _context = context;
        }

        public Task<int> GetLastOrderNumber()
        {
            return _context.Orders.OrderByDescending(o => o.OrderNumber).Select(o => o.OrderNumber).FirstOrDefaultAsync();
        }

        public void Update(Order entity)
        {
            _context.Orders.Update(entity);
        }
    }
}
