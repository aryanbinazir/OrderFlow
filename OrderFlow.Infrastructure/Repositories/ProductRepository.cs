using OrderFlow.Application.Helper.Attributes;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Context;
using OrderFlow.Infrastructure.Repositories.IRepositories;

namespace OrderFlow.Infrastructure.Repositories
{
    [Scoped]
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly OrderFlowContext _context;
        public ProductRepository(OrderFlowContext context) : base(context)
        {
            _context = context;
        }

        public Task Update(Product entity)
        {
            _context.Products.Update(entity);
            return Task.CompletedTask;
        }
    }
}
