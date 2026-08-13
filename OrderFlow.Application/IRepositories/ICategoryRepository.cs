using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Repositories.IRepositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        void Update(Category entity);
    }
}
