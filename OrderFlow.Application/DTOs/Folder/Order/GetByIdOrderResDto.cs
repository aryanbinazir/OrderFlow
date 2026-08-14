using System;
using System.Collections.Generic;

namespace OrderFlow.Application.DTOs.Folder.Order
{
    public class GetByIdOrderResDto
    {
        public Guid UserId { get; set; }
        public int OrderNumber { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public List<GetByIdOrderResDto_Item> Items { get; set; } = new();
        public DateTime? CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
    }

    public class GetByIdOrderResDto_Item
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
