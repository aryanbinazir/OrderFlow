using System;
using System.Collections.Generic;

namespace OrderFlow.Application.DTOs.Folder.Order
{
    public class UpdateOrderReqDto
    {
        public string? ShippingAddress { get; set; }
        public List<CreateOrderReqDto_Item>? ItemsToAdd { get; set; }
        public List<Guid>? ItemsToRemove { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
