using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Application.DTOs.Folder.Category
{
    public class CreateCategoryReqDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
