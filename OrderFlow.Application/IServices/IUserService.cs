using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OrderFlow.Application.DTOs.Folder.User;

namespace OrderFlow.Application.IServices
{
    public interface IUserService
    {
        Task<CreateUserResDto> Create(CreateUserReqDto dto, CancellationToken cancellationToken = default);
        Task<GetByIdUserResDto> GetById(Guid id, CancellationToken cancellationToken = default);
        Task<GetAllUserResDto> List(CancellationToken cancellationToken = default);
        Task<UpdateUserResDto> UpdateDisplayName(Guid id, UpdateUserReqDto dto, CancellationToken cancellationToken = default);
        Task<bool> PromoteToAdmin(PromoteToAdminReqDto dto, CancellationToken cancellationToken = default);
        Task<bool> Delete(Guid id, CancellationToken cancellationToken = default);
    }
}
