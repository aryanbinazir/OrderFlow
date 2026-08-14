using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Helper.Attributes;
using OrderFlow.Application.IPatterns;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Context;
using OrderFlow.Infrastructure.Repositories.IRepositories;

namespace OrderFlow.Infrastructure.Patterns;

[Scoped]
public class UnitOfWork(OrderFlowContext context, IServiceProvider serviceProvider) : IUnitOfWork
{
    private IOrderRepository? _orderRepository;
    public IOrderRepository OrderRepository =>
        _orderRepository ??= serviceProvider.GetRequiredService<IOrderRepository>();

    private IProductRepository? _productRepository;
    public IProductRepository ProductRepository =>
        _productRepository ??= serviceProvider.GetRequiredService<IProductRepository>();

    private ICategoryRepository? _categoryRepository;
    public ICategoryRepository CategoryRepository =>
        _categoryRepository ??= serviceProvider.GetRequiredService<ICategoryRepository>();

    private IUserRepository? _userRepository;
    public IUserRepository UserRepository =>
        _userRepository ??= serviceProvider.GetRequiredService<IUserRepository>();

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.Now;

        var createdGuid = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity as BaseEntity<Guid>)
            .Where(e => e is not null);

        var createdLong = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity as BaseEntity<long>)
            .Where(e => e is not null);

        var createdInt = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity as BaseEntity<int>)
            .Where(e => e is not null);

        foreach (var e in createdGuid) e.CreateDate = now;
        foreach (var e in createdLong) e.CreateDate = now;
        foreach (var e in createdInt) e.CreateDate = now;

        await context.SaveChangesAsync(cancellationToken);
    }
}
