using OrderFlow.Application.DTOs.Folder.Category;
using OrderFlow.Application.IPatterns;
using OrderFlow.Application.IServices;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateCategoryResDto> Create(CreateCategoryReqDto dto, CancellationToken cancellationToken = default)
        {
            // duplicate name check
            if (await _unitOfWork.CategoryRepository.Any(c => c.Name == dto.Name))
                throw new DomainValidationException("A category with the same name already exists.");

            var category = Category.Create(dto.Name, dto.Description, dto.ParentCategoryId, dto.CreatedBy);
            await _unitOfWork.CategoryRepository.Add(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new CreateCategoryResDto { Id = category.Id };
        }

        public async Task<GetByIdCategoryResDto> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            // include children and products for read operations
            var category = await _unitOfWork.CategoryRepository.Get(c => c.Id == id, includeProperties: "Children, Products", tracked: false);
            if (category is null) throw new DomainValidationException("Category not found.");
            return new GetByIdCategoryResDto()
            {
                Id = category.Id,
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
                CreatedAt = category.CreateDate
            };
        }

        public async Task<GetAllCategoryResDto> GetAll(CancellationToken cancellationToken = default)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAll(includeProperties: "Children, Products", tracked: false);
            return new GetAllCategoryResDto()
            {
                Categories = categories.Select(c => new GetAllCategoryResDto_Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ParentId = c.ParentId
                }).ToList()
            };
        }

        public async Task<UpdateCategoryResDto> Update(Guid id, UpdateCategoryReqDto dto, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.CategoryRepository.Get(c => c.Id == id, tracked: true);
            if (category is null) throw new DomainValidationException("Category not found.");

            // Business rules (delegated to entity)
            category.Update(dto.Name, dto.Description, dto.ParentId, dto.ModifiedBy);

            await _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateCategoryResDto { Id = category.Id };
        }

        public async Task Delete(DeleteCategoryReqDto dto, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.CategoryRepository.Get(c => c.Id == dto.CategoryId, includeProperties: "Products", tracked: true);
            if (category is null) throw new DomainValidationException("Category not found.");

            // Prevent deleting when products exist
            if (category.Products != null && category.Products.Any())
                throw new DomainValidationException("Cannot delete category that has products.");

            category.SoftDelete(dto.ModifiedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
