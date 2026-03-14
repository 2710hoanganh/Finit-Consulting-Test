using MediatR;
using TechnicalTest.Api.DTOs.ProductDTO.Response;
using TechnicalTest.Api.Features.Categories.Queries;
using TechnicalTest.Api.Services.Interfaces;

namespace TechnicalTest.Api.DTOs.CategoryDTO.Request;

public class GetProductWithinCategoryQueryHandler : IRequestHandler<GetProductWithinCategoryQuery, (IEnumerable<ProductResponse> Items, int TotalCount)>
{
    private readonly IProductService _productService;

    public GetProductWithinCategoryQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<(IEnumerable<ProductResponse> Items, int TotalCount)> Handle(GetProductWithinCategoryQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetPagedWithCategoryAsync(request.PageNumber,
                                    request.PageSize,
                                    request.SearchTerm ?? string.Empty,
                                    request.CategoryId ?? 0);
    }
}
