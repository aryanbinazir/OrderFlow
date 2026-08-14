using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Repositories.IRepositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task Update(User entity);
    }
}
