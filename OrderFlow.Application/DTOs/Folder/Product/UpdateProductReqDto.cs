using System;

namespace OrderFlow.Application.DTOs.Folder.Product
{
    public class UpdateProductReqDto
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? SKU { get; set; }
        public Guid? CategoryId { get; set; }
        public string? Description { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
