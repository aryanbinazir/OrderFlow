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
        Task<GetAllUserResDto> GetAll(CancellationToken cancellationToken = default);
        Task<UpdateUserResDto> UpdateDisplayName(Guid id, UpdateUserReqDto dto, CancellationToken cancellationToken = default);
        Task PromoteToAdmin(PromoteUserReqDto dto, CancellationToken cancellationToken = default);
        Task Delete(DeleteUserReqDto dto, CancellationToken cancellationToken = default);
    }
}
