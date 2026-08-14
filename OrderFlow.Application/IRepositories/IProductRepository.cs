using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Repositories.IRepositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task Update(Product entity);
    }
}

