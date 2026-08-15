using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTOs.Folder.Category;
using OrderFlow.Application.IServices;

namespace OrderFlow.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoryController(IServiceBase serviceBase) : ControllerBase
    {
        [HttpGet("listing")]
        public async Task<GetAllCategoryResDto> ListCategory(CancellationToken cancellationToken)
        {
            return await serviceBase.CategoryService.List(cancellationToken);
        }

        [HttpGet("listing/{id}")]
        public async Task<GetByIdCategoryResDto> GetCategoryById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return await serviceBase.CategoryService.GetById(id, cancellationToken);
        }

        [HttpPost("create")]
        public async Task<CreateCategoryResDto> CreateCategory([FromBody] CreateCategoryReqDto dto, CancellationToken cancellationToken)
        {
            return await serviceBase.CategoryService.Create(dto, cancellationToken);
        }

        [HttpPut("update/{id}")]
        public async Task<UpdateCategoryResDto> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryReqDto dto, CancellationToken cancellationToken)
        {
            return await serviceBase.CategoryService.Update(id, dto, cancellationToken);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await serviceBase.CategoryService.Delete(id, cancellationToken);
            return result ? Ok() : NotFound();
        }
    }
}
