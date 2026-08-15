using OrderFlow.Application.DTOs.Lookup;
using OrderFlow.Application.Helper.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Application.Utils
{
    public static class LookupUtils
    {
        public static Task<LookupListResDto> GetLookupList<TEnum>(CancellationToken cancellationToken)
            where TEnum : Enum
        {
            var items = Enum
                .GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new LookupListItemResDto()
                {
                    Id = Convert.ToInt64(e),
                    Name = e.ToString(),
                    FarsiName = e.GetEnumDescription()
                })
                .OrderBy(x => x.Id)
                .ToList();

            return Task.FromResult(new LookupListResDto { Items = items });
        }

    }
}
