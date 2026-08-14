using System;
using System.Threading;
using System.Threading.Tasks;
using OrderFlow.Application.DTOs.Folder.Order;

namespace OrderFlow.Application.IServices
{
    public interface IOrderService
    {
        Task<CreateOrderResDto> Create(CreateOrderReqDto dto, CancellationToken cancellationToken = default);
        Task<GetByIdOrderResDto> GetById(Guid id, CancellationToken cancellationToken = default);
        Task<GetAllOrderResDto> GetAll(CancellationToken cancellationToken = default);
        Task Delete(DeleteOrderReqDto dto, CancellationToken cancellationToken = default);
    }
}
