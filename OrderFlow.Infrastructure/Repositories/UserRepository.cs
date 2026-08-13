using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Context;
using OrderFlow.Infrastructure.Repositories.IRepositories;

namespace OrderFlow.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly OrderFlowContext _context;
        public UserRepository(OrderFlowContext context) : base(context)
        {
            _context = context;
        }

        public void Update(User entity)
        {
            _context.Users.Update(entity);
        }
    }
}
