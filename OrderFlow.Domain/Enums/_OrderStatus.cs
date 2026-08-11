using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Domain.Enums
{
    public enum _OrderStatus
    {
        [Description("پیش‌نویس")]
        Draft = 1,

        [Description("تأیید شده")]
        Confirmed = 2,

        [Description("لغو شده")]
        Cancelled = 3
    }
}
