using System;

namespace OrderFlow.Application.DTOs.Folder.User
{
    public class CreateUserReqDto
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? DisplayName { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
