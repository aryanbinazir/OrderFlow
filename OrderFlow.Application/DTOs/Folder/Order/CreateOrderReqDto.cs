using System;
using System.Collections.Generic;

namespace OrderFlow.Application.DTOs.Folder.Order
{
    public class CreateOrderReqDto
    {
        public Guid UserId { get; set; }
        public List<CreateOrderReqDto_Item> Items { get; set; } = new();
        public Guid? CreatedBy { get; set; }
    }

    public class CreateOrderReqDto_Item
    {
        public Guid ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
