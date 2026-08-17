using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Repositories.IRepositories
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<int> GetLastOrderNumber();
    }
}
