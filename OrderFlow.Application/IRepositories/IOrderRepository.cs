using OrderFlow.Domain.Entities;
using System.Collections.Generic;

namespace OrderFlow.Infrastructure.Repositories.IRepositories
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        void Update(Order entity);

    }
}
