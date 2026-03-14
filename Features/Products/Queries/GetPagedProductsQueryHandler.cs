using MediatR;
using TechnicalTest.Api.DTOs.ProductDTO.Response;
using TechnicalTest.Api.Services.Interfaces;

namespace TechnicalTest.Api.Features.Products.Queries;

public class GetPagedProductsQueryHandler : IRequestHandler<GetPagedProductsQuery, (IEnumerable<ProductResponse> Items, int TotalCount)>
{
    private readonly IProductService _productService;

    public GetPagedProductsQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<(IEnumerable<ProductResponse> Items, int TotalCount)> Handle(GetPagedProductsQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetPagedProductsAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.CategoryId);
    }
}
