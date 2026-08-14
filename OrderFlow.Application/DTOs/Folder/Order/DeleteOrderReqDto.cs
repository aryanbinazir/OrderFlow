using System;

namespace OrderFlow.Application.DTOs.Folder.Order
{
    public class DeleteOrderReqDto
    {
        public Guid OrderId { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
