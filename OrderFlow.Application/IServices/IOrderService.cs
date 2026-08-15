using OrderFlow.Application.DTOs.Folder.Order;

namespace OrderFlow.Application.IServices
{
    public interface IOrderService
    {
        Task<CreateOrderResDto> Create(CreateOrderReqDto dto, CancellationToken cancellationToken = default);
        Task<GetByIdOrderResDto> GetById(Guid id, CancellationToken cancellationToken = default);
        Task<GetAllOrderResDto> List(CancellationToken cancellationToken = default);
        Task<bool> Confirm(ConfirmOrderReqDto dto, CancellationToken cancellationToken = default);
        Task<bool> Cancel(CancelOrderReqDto dto, CancellationToken cancellationToken = default);
        Task<bool> Delete(Guid id, CancellationToken cancellationToken = default);
    }
}
