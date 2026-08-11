using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities
{
    public class UserRole : BaseLookupEntity<_UserRole>
    {
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
