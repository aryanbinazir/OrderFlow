using OrderFlow.Infrastructure.Repositories.IRepositories;

namespace OrderFlow.Application.IPatterns;

public interface IUnitOfWork
{
    IOrderRepository OrderRepository { get; }
    IProductRepository ProductRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IUserRepository UserRepository { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

