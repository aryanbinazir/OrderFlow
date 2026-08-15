using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.DTOs.Folder.Category;
using OrderFlow.Application.Helper.Attributes;
using OrderFlow.Application.Helper.Exception;
using OrderFlow.Application.Helper.Exception.Enums;
using OrderFlow.Application.IPatterns;
using OrderFlow.Application.IServices;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Application.Services
{
    [Scoped]
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateCategoryResDto> Create(CreateCategoryReqDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                // duplicate name check
                if (await _unitOfWork.CategoryRepository.Any(c => c.Name == dto.Name))
                {
                    throw new InternalServerErrorException(
                    "A category with the same name already exists.",
                    _CriticalLevel.Three);
                }

                var category = Category.Create(dto.Name, dto.Description, dto.ParentId, dto.CreatedBy);
                
                if (dto.ParentId != null)
                {
                    var parent = await _unitOfWork.CategoryRepository.Get(c => c.Id == dto.ParentId);
                    parent.AddChild(category);
                }

                await _unitOfWork.CategoryRepository.Add(category);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return new CreateCategoryResDto { Id = category.Id };
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

        public async Task<GetByIdCategoryResDto> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.Get(
                    c => c.Id == id,
                    [ x => x.Products, x => x.Parent, x => x.Children],
                    tracked: false);
                if (category is null)
                {
                    throw new InternalServerErrorException(
                    "Category not found",
                    _CriticalLevel.Three);
                }
                return new GetByIdCategoryResDto()
                {
                    Name = category.Name,
                    Description = category.Description,
                    ParentId = category.ParentId,
                    Products = category.Products?.Select(p => new GetByIdCategoryResDto_Products
                    {
                        Id = p.Id,
                        Name = p.Name,
                    }).ToList(),
                    Children = category.Children?.Select(c => new GetByIdCategoryResDto_Children
                    {
                        Id = c.Id,
                        Name = c.Name,
                    }).ToList(),
                    CreatedAt = category.CreateDate,
                    ModifiedAt = category.ModifiedDate
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

        public async Task<GetAllCategoryResDto> List(CancellationToken cancellationToken = default)
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository.GetAll(includes: [x => x.Products], tracked: false);
                return new GetAllCategoryResDto()
                {
                    Categories = categories.Select(c => new GetAllCategoryResDto_Category
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        ParentId = c.ParentId,
                        Products = c.Products?.Select(p => new GetAllCategoryResDto_Category_Products
                        {
                            Name = p.Name
                        }).ToList(),
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

        public async Task<UpdateCategoryResDto> Update(Guid id, UpdateCategoryReqDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.Get(c => c.Id == id, [x => x.Products], tracked: true);
                if (category is null)
                {
                    throw new InternalServerErrorException(
                    "Category not found",
                    _CriticalLevel.Three);
                }

                // duplicate name check
                if (await _unitOfWork.CategoryRepository.Any(c => c.Name == dto.Name))
                {
                    throw new InternalServerErrorException(
                    "A category with the same name already exists.",
                    _CriticalLevel.Three);
                }

                if (dto.ParentId != null)
                {
                    var parent = await _unitOfWork.CategoryRepository.Get(c => c.Id == dto.ParentId);
                    parent.AddChild(category);
                }

                // Business rules (delegated to entity)
                category.Update(dto.Name, dto.Description, dto.ParentId, dto.ModifiedBy);

                await _unitOfWork.CategoryRepository.Update(category);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new UpdateCategoryResDto { Id = category.Id };
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
                var category = await _unitOfWork.CategoryRepository.Get(
                    c => c.Id == id,
                    [x => x.Products, x => x.Children, x => x.Parent],
                    tracked: true);
                if (category is null)
                {
                    throw new InternalServerErrorException(
                    "Category not found",
                    _CriticalLevel.Three);
                }

                category.Delete();
                await _unitOfWork.CategoryRepository.Remove(category);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DomainValidationException ex)
            {
                throw new BadRequestException(
                    ex.Message,
                    _CriticalLevel.Zero);
            }
            //catch (DbUpdateException)
            //{
            //    throw new InternalServerErrorException(
            //        "Try again later",
            //        _CriticalLevel.Five);
            //}
        }
    }
}
