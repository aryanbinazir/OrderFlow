using System;

namespace OrderFlow.Application.DTOs.Folder.User
{
    public class PromoteToAdminReqDto
    {
        public Guid UserId { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
