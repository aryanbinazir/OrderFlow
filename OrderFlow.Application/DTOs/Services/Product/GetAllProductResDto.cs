using System;
using System.Collections.Generic;

namespace OrderFlow.Application.DTOs.Folder.Product
{
    public class GetAllProductResDto
    {
        public List<GetAllProductResDto_Product> Products { get; set; } = [];
    }

    public class GetAllProductResDto_Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
