using System;
using System.Collections.Generic;

namespace OrderFlow.Application.DTOs.Folder.Order
{
    public class GetAllOrderResDto
    {
        public List<GetAllOrderResDto_Order> Orders { get; set; }
    }

    public class GetAllOrderResDto_Order
    {
        public Guid Id { get; set; }
        public int OrderNumber { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public Guid UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
