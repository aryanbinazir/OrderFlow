using OrderFlow.Application.DTOs.Folder.Product;

namespace OrderFlow.Application.IServices
{
    public interface IProductService
    {
        Task<CreateProductResDto> Create(CreateProductReqDto dto, CancellationToken cancellationToken = default);
        Task<GetByIdProductResDto> GetById(Guid id, CancellationToken cancellationToken = default);
        Task<GetAllProductResDto> GetAll(CancellationToken cancellationToken = default);
        Task<UpdateProductResDto> Update(Guid id, UpdateProductReqDto dto, CancellationToken cancellationToken = default);
        Task IncreaseProductStock(IncreaseProductStockReqDto dto, CancellationToken cancellationToken = default);
        Task DecreaseProductStock(DecreaseProductStockReqDto dto, CancellationToken cancellationToken = default);
        Task Delete(DeleteProductReqDto dto, CancellationToken cancellationToken = default);
    }
}
