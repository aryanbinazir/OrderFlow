using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Repositories.IRepositories
{
    public interface IOrderItemRepository : IBaseRepository<OrderItem>
    {
        void Update(OrderItem entity);

    }
}
