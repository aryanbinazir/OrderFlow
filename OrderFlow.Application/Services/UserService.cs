using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.DTOs.Folder.User;
using OrderFlow.Application.Helper.Attributes;
using OrderFlow.Application.Helper.Exception;
using OrderFlow.Application.Helper.Exception.Enums;
using OrderFlow.Application.IPatterns;
using OrderFlow.Application.IServices;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Domain.Exceptions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderFlow.Application.Services
{
    [Scoped]
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateUserResDto> Create(CreateUserReqDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                // duplicate email
                if (await _unitOfWork.UserRepository.Any(u => u.Email == dto.Email))
                {
                    throw new InternalServerErrorException(
                     "A user with the same email is already exist",
                    _CriticalLevel.Three);
                }

                var hashPassword = HashUtils.ComputeSha256(dto.PasswordHash);

                var user = User.Create(dto.Email, hashPassword, dto.DisplayName, dto.CreatedBy);
                await _unitOfWork.UserRepository.Add(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return new CreateUserResDto { Id = user.Id };
            }
            catch (DomainValidationException ex)
            {
                throw new BadRequestException(
                    ex.Message,
                    _CriticalLevel.Zero);
            }
            catch (DbUpdateException)
            {
                throw new InternalServerErrorException(
                    "Try again later",
                    _CriticalLevel.Five);
            }
        }

        public async Task<GetByIdUserResDto> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.Get(u => u.Id == id, [x => x.Orders], tracked: false);
                if (user is null)
                {
                    throw new InternalServerErrorException(
                    "User Not found",
                    _CriticalLevel.Three);
                }

                return new GetByIdUserResDto
                {
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    RoleName = user.RoleId.ToString(),
                    CreatedAt = user.CreateDate,
                    ModifiedAt = user.ModifiedDate,
                    Orders = user.Orders?.Select(o => new GetByIdUserResDto_Orders
                    {
                        OrderNumber = o.OrderNumber,
                    }).ToList() ?? new List<GetByIdUserResDto_Orders>()
                };
            }
            catch (DomainValidationException ex)
            {
                throw new BadRequestException(
                    ex.Message,
                    _CriticalLevel.Zero);
            }
            catch (DbUpdateException)
            {
                throw new InternalServerErrorException(
                    "Try again later",
                    _CriticalLevel.Five);
            }
        }

        public async Task<GetAllUserResDto> List(CancellationToken cancellationToken = default)
        {
            try
            { var users = await _unitOfWork.UserRepository.GetAll(includes: [x => x.Orders], tracked: false);
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
            catch (DomainValidationException ex)
            {
                throw new BadRequestException(
                    ex.Message,
                    _CriticalLevel.Zero);
            }
            catch (DbUpdateException)
            {
                throw new InternalServerErrorException(
                    "Try again later",
                    _CriticalLevel.Five);
            }
        }
        public async Task<UpdateUserResDto> UpdateDisplayName(Guid id, UpdateUserReqDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.Get(u => u.Id == id, tracked: true);
                if (user is null)
                {
                    throw new InternalServerErrorException(
                    "User Not found",
                    _CriticalLevel.Three);
                }

                user.UpdateDisplayName(dto.DisplayName, dto.ModifiedBy);

                await _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new UpdateUserResDto { Id = user.Id };
            }
            catch (DomainValidationException ex)
            {
                throw new BadRequestException(
                    ex.Message,
                    _CriticalLevel.Zero);
            }
            catch (DbUpdateException)
            {
                throw new InternalServerErrorException(
                    "Try again later",
                    _CriticalLevel.Five);
            }
        }

        public async Task<bool> PromoteToAdmin(PromoteToAdminReqDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.Get(u => u.Id == dto.UserId, tracked: true);
                if (user is null)
                {
                    throw new InternalServerErrorException(
                    "User Not found",
                    _CriticalLevel.Three);
                }

                user.PromoteToAdmin(user.Id, dto.ModifiedBy);
                await _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DomainValidationException ex)
            {
                throw new BadRequestException(
                    ex.Message,
                    _CriticalLevel.Zero);
            }
            catch (DbUpdateException)
            {
                throw new InternalServerErrorException(
                    "Try again later",
                    _CriticalLevel.Five);
            }
        }

        public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.Get(u => u.Id == id, [x => x.Orders], tracked: true);
                if (user is null)
                {
                    throw new InternalServerErrorException(
                    "User Not found",
                    _CriticalLevel.Three);
                }

                await _unitOfWork.UserRepository.Remove(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DomainValidationException ex)
            {
                throw new BadRequestException(
                    ex.Message,
                    _CriticalLevel.Zero);
            }
            catch (DbUpdateException)
            {
                throw new InternalServerErrorException(
                    "Try again later",
                    _CriticalLevel.Five);
            }
        }
    }
}
