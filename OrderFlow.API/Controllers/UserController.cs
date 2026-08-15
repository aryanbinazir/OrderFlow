using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTOs.Folder.User;
using OrderFlow.Application.DTOs.Lookup;
using OrderFlow.Application.IServices;
using OrderFlow.Application.Utils;
using OrderFlow.Domain.Enums;

namespace OrderFlow.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController(IServiceBase serviceBase) : ControllerBase
    {
        [HttpGet("listing")]
        public async Task<GetAllUserResDto> ListUsers(CancellationToken cancellationToken)
        {
            return await serviceBase.UserService.List(cancellationToken);
        }

        [HttpGet("listing/{id}")]
        public async Task<GetByIdUserResDto> GetUserById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return await serviceBase.UserService.GetById(id, cancellationToken);
        }

        [HttpPost("create")]
        public async Task<CreateUserResDto> CreateUser([FromBody] CreateUserReqDto dto, CancellationToken cancellationToken)
        {
            return await serviceBase.UserService.Create(dto, cancellationToken);
        }

        [HttpPatch("update-display-name/{id}")]
        public async Task<UpdateUserResDto> UpdateUserDisplayName([FromRoute] Guid id, [FromBody] UpdateUserReqDto dto, CancellationToken cancellationToken)
        {
            return await serviceBase.UserService.UpdateDisplayName(id, dto, cancellationToken);
        }

        [HttpPost("PromoteToAdmin")]
        public async Task<IActionResult> PromoteToAdmin([FromBody] PromoteToAdminReqDto dto, CancellationToken cancellationToken)
        {
            var result = await serviceBase.UserService.PromoteToAdmin(dto, cancellationToken);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await serviceBase.UserService.Delete(id, cancellationToken);
            return result ? Ok() : NotFound();
        }

        [HttpGet("user-role/list")]
        public Task<LookupListResDto> UserRoleList(CancellationToken cancellationToken)
            => LookupUtils.GetLookupList<_UserRole>(cancellationToken);
    }
}
