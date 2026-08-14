using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Application.DTOs.Folder.Category
{
    public class UpdateCategoryReqDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? ModifiedBy { get; set; }

    }
}
