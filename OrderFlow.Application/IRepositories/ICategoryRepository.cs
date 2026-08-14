using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Repositories.IRepositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task Update(Category entity);
    }
}
