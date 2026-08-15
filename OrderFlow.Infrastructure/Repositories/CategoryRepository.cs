using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Helper.Attributes;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Context;
using OrderFlow.Infrastructure.Repositories.IRepositories;

namespace OrderFlow.Infrastructure.Repositories
{
    [Scoped]
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly OrderFlowContext _context;
        public CategoryRepository(OrderFlowContext context) : base(context)
        {
            _context = context;
        }

        public Task Update(Category entity)
        {
            _context.Categories.Update(entity);
            return Task.CompletedTask;
        }
    }
}
