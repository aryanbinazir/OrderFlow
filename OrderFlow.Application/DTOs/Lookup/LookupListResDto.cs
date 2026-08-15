using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Application.DTOs.Lookup
{
    public class LookupListResDto
    {
        public List<LookupListItemResDto> Items { get; set; }
    }

    public class LookupListItemResDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FarsiName { get; set; }

    }
}
