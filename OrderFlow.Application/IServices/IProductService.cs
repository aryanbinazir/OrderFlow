using OrderFlow.Application.DTOs.Folder.Product;

namespace OrderFlow.Application.IServices
{
    public interface IProductService
    {
        Task<CreateProductResDto> Create(CreateProductReqDto dto, CancellationToken cancellationToken = default);
        Task<GetByIdProductResDto> GetById(Guid id, CancellationToken cancellationToken = default);
        Task<GetAllProductResDto> List(CancellationToken cancellationToken = default);
        Task<UpdateProductResDto> Update(Guid id, UpdateProductReqDto dto, CancellationToken cancellationToken = default);
        Task<bool> IncreaseProductStock(IncreaseProductStockReqDto dto, CancellationToken cancellationToken = default);
        Task<bool> DecreaseProductStock(DecreaseProductStockReqDto dto, CancellationToken cancellationToken = default);
        Task<bool> Delete(Guid id, CancellationToken cancellationToken = default);
    }
}
