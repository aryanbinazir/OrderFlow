using System;

namespace OrderFlow.Application.DTOs.Folder.Product
{
    public class DeleteProductReqDto
    {
        public Guid ProductId { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
