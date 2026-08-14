using OrderFlow.Application.DTOs.Folder.Category;
using OrderFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OrderFlow.Application.IServices
{
    public interface ICategoryService
    {
        Task<CreateCategoryResDto> Create(CreateCategoryReqDto dto, CancellationToken cancellationToken = default);
        Task<GetByIdCategoryResDto> GetById(Guid id, CancellationToken cancellationToken = default);
        Task<GetAllCategoryResDto> GetAll(CancellationToken cancellationToken = default);
        Task<UpdateCategoryResDto> Update(Guid id, UpdateCategoryReqDto dto, CancellationToken cancellationToken = default);
        Task Delete(DeleteCategoryReqDto dto, CancellationToken cancellationToken = default);
    }
}
