using System;

namespace OrderFlow.Application.DTOs.Folder.Order
{
    public class ConfirmOrderReqDto
    {
        public Guid OrderId { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
