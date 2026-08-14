using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Application.DTOs.Folder.Category
{
    public class GetAllCategoryResDto
    {
        public List<GetAllCategoryResDto_Category> Categories { get; set; }
    }

    public class GetAllCategoryResDto_Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
    }
}
