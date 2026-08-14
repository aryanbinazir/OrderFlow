using System;
using System.Collections.Generic;

namespace OrderFlow.Application.DTOs.Folder.Product
{
    public class GetByIdProductResDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Guid? CategoryId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
