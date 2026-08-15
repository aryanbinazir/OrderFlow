using OrderFlow.Application.Helper.Attributes;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Context;
using OrderFlow.Infrastructure.Repositories.IRepositories;

namespace OrderFlow.Infrastructure.Repositories
{
    [Scoped]
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly OrderFlowContext _context;
        public UserRepository(OrderFlowContext context) : base(context)
        {
            _context = context;
        }

        public Task Update(User entity)
        {
            _context.Users.Update(entity);
            return Task.CompletedTask;
        }
    }
}
