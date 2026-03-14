using MediatR;
using TechnicalTest.Api.DTOs.CategoryDTO.Response;
using TechnicalTest.Api.Services.Interfaces;

namespace TechnicalTest.Api.Features.Categories.Queries;

public class GetPagedCategoriesQueryHandler : IRequestHandler<GetPagedCategoriesQuery, (IEnumerable<CategoryResponse> Items, int TotalCount)>
{
    private readonly ICategoryService _categoryService;

    public GetPagedCategoriesQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<(IEnumerable<CategoryResponse> Items, int TotalCount)> Handle(GetPagedCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _categoryService.GetPagedAsync(request.PageNumber, request.PageSize);
    }
}
