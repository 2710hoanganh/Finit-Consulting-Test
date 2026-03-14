using MediatR;
using TechnicalTest.Api.DTOs.ProductDTO.Response;
using TechnicalTest.Api.Services.Interfaces;

namespace TechnicalTest.Api.DTOs.CategoryDTO.Request;

public class SearchProductQueryHandler : IRequestHandler<SearchProductQuery, IEnumerable<ProductResponse>>
{
    private readonly IProductService _productService;

    public SearchProductQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IEnumerable<ProductResponse>> Handle(SearchProductQuery request, CancellationToken cancellationToken)
    {
        var result = await _productService.SearchProductAsync(request.CategoryId,
                    request.SearchTerm,
                    request.CreateFrom,
                    request.CreateTo,
                    request.IsActive,
                    request.PriceFrom,
                    request.PriceTo,
                    request.StockFrom,
                    request.StockTo);
        return result.Items;
    }
}