using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OrderFlow.Application.DTOs.Folder.User;
using OrderFlow.Application.Helper.Attributes;
using OrderFlow.Application.IPatterns;
using OrderFlow.Application.IServices;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateUserResDto> Create(CreateUserReqDto dto, CancellationToken cancellationToken = default)
        {
            // duplicate email
            if (await _unitOfWork.UserRepository.Any(u => u.Email == dto.Email))
                throw new DomainValidationException("A user with the same email already exists.");

            var hashPassword = HashUtils.ComputeSha256(dto.PasswordHash);

            var user = User.Create(dto.Email, hashPassword, dto.DisplayName, dto.CreatedBy);
            await _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new CreateUserResDto { Id = user.Id };
        }

        public async Task<GetByIdUserResDto> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.UserRepository.Get(u => u.Id == id, includeProperties: "UserRole", tracked: false);
            if (user is null) throw new DomainValidationException("User not found.");

            return new GetByIdUserResDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                RoleName = user.RoleId.ToString(),
                CreatedAt = user.CreateDate,
                Orders = user.Orders?.Select(o => new GetByIdUserResDto_Orders
                {
                    OrderNumber = o.OrderNumber,
                }).ToList() ?? new List<GetByIdUserResDto_Orders>()
            };
        }

        public async Task<GetAllUserResDto> GetAll(CancellationToken cancellationToken = default)
        {
            var users = await _unitOfWork.UserRepository.GetAll(includeProperties: "UserRole", tracked: false);
            return new GetAllUserResDto
            {
                Users = users.Select(u => new GetAllUserResDto_User
                {
                    Id = u.Id,
                    Email = u.Email,
                    DisplayName = u.DisplayName,
                    Role = u.RoleId.ToString()
                }).ToList()
            };
        }

        public async Task<UpdateUserResDto> UpdateDisplayName(Guid id, UpdateUserReqDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.UserRepository.Get(u => u.Id == id, tracked: true);
            if (user is null) throw new DomainValidationException("User not found.");

            user.UpdateDisplayName(dto.DisplayName, dto.ModifiedBy);

            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateUserResDto { Id = user.Id };
        }

        public async Task PromoteToAdmin(PromoteUserReqDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.UserRepository.Get(u => u.Id == dto.UserId, tracked: true);
            if (user is null) throw new DomainValidationException("User not found.");

            user.PromoteToAdmin(user.Id, dto.ModifiedBy);
            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(DeleteUserReqDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.UserRepository.Get(u => u.Id == dto.UserId, tracked: true);
            if (user is null) throw new DomainValidationException("User not found.");

            user.SoftDelete(dto.ModifiedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
