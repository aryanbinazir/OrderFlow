using System;

namespace OrderFlow.Application.DTOs.Folder.User
{
    public class GetByIdUserResDto
    {
        public string Email { get; set; }
        public string? DisplayName { get; set; }
        public string? RoleName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public List<GetByIdUserResDto_Orders> Orders { get; set; }
    }

    public class GetByIdUserResDto_Orders
    {
        public int OrderNumber { get; set; }
    }
}
