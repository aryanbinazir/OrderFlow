using System;

namespace OrderFlow.Application.DTOs.Folder.User
{
    public class DeleteUserReqDto
    {
        public Guid UserId { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
