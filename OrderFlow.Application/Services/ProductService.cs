using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.DTOs.Folder.Product;
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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateProductResDto> Create(CreateProductReqDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Optionally validate category exists
                if (dto.CategoryId.HasValue)
                {
                    var category = await _unitOfWork.CategoryRepository.Get(c => c.Id == dto.CategoryId, tracked: false);
                    if (category is null)
                    {
                        throw new InternalServerErrorException(
                        "Category not found",
                        _CriticalLevel.Three);
                    }
                }

                var product = Product.Create(dto.Name, dto.Price, dto.CategoryId, dto.Stock, dto.Description, dto.CreatedBy);
                await _unitOfWork.ProductRepository.Add(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return new CreateProductResDto { Id = product.Id };
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

        public async Task<GetByIdProductResDto> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.Get(
                    p => p.Id == id,
                    [x => x.Category, x => x.orderItems],
                    tracked: false);
                if (product is null)
                {
                    throw new InternalServerErrorException(
                    "Product not found",
                    _CriticalLevel.Three);
                }

                return new GetByIdProductResDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    CategoryId = product.CategoryId,
                    CreatedAt = product.CreateDate,
                    ModifiedAt = product.ModifiedDate,
                    CreatedBy = product.CreateById
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

        public async Task<GetAllProductResDto> List(CancellationToken cancellationToken = default)
        {
            try
            {
                var products = await _unitOfWork.ProductRepository.GetAll(
                    includes: [x => x.Category, x => x.orderItems],
                    tracked: false);
                return new GetAllProductResDto
                {
                    Products = products.Select(p => new GetAllProductResDto_Product
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Stock = p.Stock,
                        CategoryId = p.CategoryId
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

        public async Task<UpdateProductResDto> Update(Guid id, UpdateProductReqDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.Get(
                    p => p.Id == id,
                    [x => x.Category],
                    tracked: true);
                if (product is null)
                {
                    throw new InternalServerErrorException(
                    "Product not found",
                    _CriticalLevel.Three);
                }
                // Delegate validations and state changes to entity methods
                product.Update(dto.Name, dto.Price, dto.CategoryId, dto.Description, dto.ModifiedBy);

                await _unitOfWork.ProductRepository.Update(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new UpdateProductResDto { Id = product.Id };
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

        public async Task<bool> IncreaseProductStock(IncreaseProductStockReqDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.Get(p => p.Id == dto.ProductId, tracked: true);
                if (product is null)
                {
                    throw new InternalServerErrorException(
                    "Product not found",
                    _CriticalLevel.Three);
                }

                product.IncreaseStock(dto.Amount, null);
                await _unitOfWork.ProductRepository.Update(product);
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

        public async Task<bool> DecreaseProductStock(DecreaseProductStockReqDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.Get(p => p.Id == dto.ProductId, tracked: true);
                if (product is null)
                {
                    throw new InternalServerErrorException(
                    "Product not found",
                    _CriticalLevel.Three);
                }

                product.DecreaseStock(dto.Amount, null);
                await _unitOfWork.ProductRepository.Update(product);
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
                var product = await _unitOfWork.ProductRepository.Get(
                    p => p.Id == id,
                    [x => x.Category, x => x.orderItems],
                    tracked: true);
                if (product is null)
                {
                    throw new InternalServerErrorException(
                    "Product not found",
                    _CriticalLevel.Three);
                }

                await _unitOfWork.ProductRepository.Remove(product);
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
