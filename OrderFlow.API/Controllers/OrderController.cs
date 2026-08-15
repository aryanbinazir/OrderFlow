using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTOs.Folder.Order;
using OrderFlow.Application.DTOs.Lookup;
using OrderFlow.Application.IServices;
using OrderFlow.Application.Utils;
using OrderFlow.Domain.Enums;

namespace OrderFlow.API.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController(IServiceBase serviceBase) : ControllerBase
    {
        [HttpGet("listing")]
        public async Task<GetAllOrderResDto> ListOrder(CancellationToken cancellationToken)
        {
            return await serviceBase.OrderService.List(cancellationToken);
        }

        [HttpGet("listing/{id}")]
        public async Task<GetByIdOrderResDto> GetOrderById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return await serviceBase.OrderService.GetById(id, cancellationToken);
        }

        [HttpPost("create")]
        public async Task<CreateOrderResDto> CreateOrder([FromBody] CreateOrderReqDto dto, CancellationToken cancellationToken)
        {
            return await serviceBase.OrderService.Create(dto, cancellationToken);
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmOrder([FromBody] ConfirmOrderReqDto dto, CancellationToken cancellationToken)
        {
            var result = await serviceBase.OrderService.Confirm(dto, cancellationToken);
            return result ? Ok() : NotFound();
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderReqDto dto, CancellationToken cancellationToken)
        {
            var result = await serviceBase.OrderService.Cancel(dto, cancellationToken);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await serviceBase.OrderService.Delete(id, cancellationToken);
            return result ? Ok() : NotFound();
        }

        [HttpGet("order-status/list")]
        public Task<LookupListResDto> OrderStatusList(CancellationToken cancellationToken)
            => LookupUtils.GetLookupList<_OrderStatus>(cancellationToken);
    }
}
