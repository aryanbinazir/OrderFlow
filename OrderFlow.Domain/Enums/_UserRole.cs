using System.ComponentModel;

namespace OrderFlow.Domain.Enums
{
    public enum  _UserRole
    {
        [Description("مدیر")]
        Admin = 1,

        [Description("مشتری")]
        Client = 2,
    }
}
