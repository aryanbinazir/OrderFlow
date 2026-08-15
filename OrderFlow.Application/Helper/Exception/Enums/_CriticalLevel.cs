using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Application.Helper.Exception.Enums
{
    public enum _CriticalLevel
    {
        [Description("Zero - No impact")]
        Zero = 0,

        [Description("One - Very Low")]
        One = 1,

        [Description("Two - Low")]
        Two = 2,

        [Description("Three - Medium")]
        Three = 3,

        [Description("Four - High")]
        Four = 4,

        [Description("Five - Critical")]
        Five = 5
    }

}
