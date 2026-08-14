using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Application.DTOs.Folder.Category
{
    public class DeleteCategoryReqDto
    {
        public Guid CategoryId { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
