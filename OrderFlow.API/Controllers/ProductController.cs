using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTOs.Folder.Category;
using OrderFlow.Application.DTOs.Folder.Product;
using OrderFlow.Application.IServices;

namespace OrderFlow.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController(IServiceBase serviceBase) : ControllerBase
    {
        [HttpGet("listing")]
        public async Task<GetAllProductResDto> ListProduct(CancellationToken cancellationToken)
        {
            return await serviceBase.ProductService.List(cancellationToken);
        }

        [HttpGet("listing/{id}")]
        public async Task<GetByIdProductResDto> GetByIdProduct([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return await serviceBase.ProductService.GetById(id, cancellationToken);
        }

        [HttpPost("create")]
        public async Task<CreateProductResDto> CreateProduct([FromBody] CreateProductReqDto dto, CancellationToken cancellationToken)
        {
            return await serviceBase.ProductService.Create(dto, cancellationToken);
        }

        [HttpPut("update/{id}")]
        public async Task<UpdateProductResDto> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductReqDto dto, CancellationToken cancellationToken)
        {
            return await serviceBase.ProductService.Update(id, dto, cancellationToken);
        }

        [HttpPost("increase-stock")]
        public async Task<IActionResult> IncreaseProductStock(IncreaseProductStockReqDto dto, CancellationToken cancellationToken)
        {
            var result = await serviceBase.ProductService.IncreaseProductStock(dto, cancellationToken);
            return result ? Ok() : NotFound();
        }

        [HttpPost("decrease-stock")]
        public async Task<IActionResult> DecreaseProductStock(DecreaseProductStockReqDto dto, CancellationToken cancellationToken)
        {
            var result = await serviceBase.ProductService.DecreaseProductStock(dto, cancellationToken);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await serviceBase.ProductService.Delete(id, cancellationToken);
            return result ? Ok() : NotFound();
        }   
    }
}
