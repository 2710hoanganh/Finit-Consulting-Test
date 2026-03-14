using TechnicalTest.Api.DTOs.CategoryDTO.Request;
using TechnicalTest.Api.DTOs.CategoryDTO.Response;

namespace TechnicalTest.Api.Services.Interfaces;

public interface ICategoryService : IBaseService<CategoryResponse, CategoryCreateRequest, CategoryUpdateRequest>
{
}
