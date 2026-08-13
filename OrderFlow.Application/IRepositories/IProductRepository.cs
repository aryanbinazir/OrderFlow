using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Repositories.IRepositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        void Update(Product entity);
    }
}

