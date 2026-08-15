using System;
using System.Collections.Generic;

namespace OrderFlow.Application.DTOs.Folder.User
{
    public class GetAllUserResDto
    {
        public List<GetAllUserResDto_User> Users { get; set; } = [];
    }

    public class GetAllUserResDto_User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string? DisplayName { get; set; }
        public string Role { get; set; }
    }
}
