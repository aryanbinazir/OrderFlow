using OrderFlow.Application.DTOs.Folder.Order;

namespace OrderFlow.Application.IServices
{
    public interface IOrderService
    {
        Task<CreateOrderResDto> Create(CreateOrderReqDto dto, CancellationToken cancellationToken = default);
        Task<GetByIdOrderResDto> GetById(Guid id, CancellationToken cancellationToken = default);
        Task<GetAllOrderResDto> GetAll(CancellationToken cancellationToken = default);
        Task Confirm(ConfirmOrderReqDto dto, CancellationToken cancellationToken = default);
        Task Canscl(CancelOrderReqDto dto, CancellationToken cancellationToken = default);
        Task Delete(DeleteOrderReqDto dto, CancellationToken cancellationToken = default);
    }
}
