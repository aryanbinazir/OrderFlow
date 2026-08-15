using System;

namespace OrderFlow.Application.DTOs.Folder.Product
{
    public class IncreaseProductStockReqDto
    {
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
