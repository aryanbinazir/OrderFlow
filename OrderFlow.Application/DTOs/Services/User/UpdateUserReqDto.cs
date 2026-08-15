using System;

namespace OrderFlow.Application.DTOs.Folder.User
{
    public class UpdateUserReqDto
    {
        public string? DisplayName { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
