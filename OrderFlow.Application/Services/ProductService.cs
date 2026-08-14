using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OrderFlow.Application.DTOs.Folder.Product;
using OrderFlow.Application.IPatterns;
using OrderFlow.Application.IServices;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateProductResDto> Create(CreateProductReqDto dto, CancellationToken cancellationToken = default)
        {
            // duplicate SKU
            if (await _unitOfWork.ProductRepository.Any(p => p.SKU == dto.SKU))
                throw new DomainValidationException("A product with the same SKU already exists.");

            // Optionally validate category exists
            if (dto.CategoryId.HasValue)
            {
                var category = await _unitOfWork.CategoryRepository.Get(c => c.Id == dto.CategoryId.Value, tracked: false);
                if (category is null) throw new DomainValidationException("Category not found.");
            }

            var product = Product.Create(dto.Name, dto.Price, dto.SKU, dto.CategoryId, dto.Stock, dto.Description, dto.CreatedBy);
            await _unitOfWork.ProductRepository.Add(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new CreateProductResDto { Id = product.Id };
        }

        public async Task<GetByIdProductResDto> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _unitOfWork.ProductRepository.Get(p => p.Id == id, includeProperties: "Category", tracked: false);
            if (product is null) throw new DomainValidationException("Product not found.");

            return new GetByIdProductResDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                CreatedAt = product.CreateDate
            };
        }

        public async Task<GetAllProductResDto> GetAll(CancellationToken cancellationToken = default)
        {
            var products = await _unitOfWork.ProductRepository.GetAll(includeProperties: "Category", tracked: false);
            return new GetAllProductResDto
            {
                Products = products.Select(p => new GetAllProductResDto_Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    SKU = p.SKU,
                    Price = p.Price,
                    Stock = p.Stock,
                    CategoryId = p.CategoryId
                }).ToList()
            };
        }

        public async Task<UpdateProductResDto> Update(Guid id, UpdateProductReqDto dto, CancellationToken cancellationToken = default)
        {
            var product = await _unitOfWork.ProductRepository.Get(p => p.Id == id, tracked: true);
            if (product is null) throw new DomainValidationException("Product not found.");

            // Delegate validations and state changes to entity methods
            product.Update(dto.Name, dto.Price, dto.SKU, dto.CategoryId, dto.Description, dto.ModifiedBy);

            await _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateProductResDto { Id = product.Id };
        }

        public async Task IncreaseProductStock(IncreaseProductStockReqDto dto, CancellationToken cancellationToken = default)
        {
            var product = await _unitOfWork.ProductRepository.Get(p => p.Id == dto.ProductId, tracked: true);
            if (product is null) throw new DomainValidationException("Product not found.");

            product.IncreaseStock(dto.Amount, null);
            await _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DecreaseProductStock(DecreaseProductStockReqDto dto, CancellationToken cancellationToken = default)
        {
            var product = await _unitOfWork.ProductRepository.Get(p => p.Id == dto.ProductId, tracked: true);
            if (product is null) throw new DomainValidationException("Product not found.");

            product.DecreaseStock(dto.Amount, null);
            await _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(DeleteProductReqDto dto, CancellationToken cancellationToken = default)
        {
            var product = await _unitOfWork.ProductRepository.Get(p => p.Id == dto.ProductId, tracked: true);
            if (product is null) throw new DomainValidationException("Product not found.");

            product.SoftDelete();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
