using MediatR;
using TechnicalTest.Api.DTOs.ProductDTO.Response;
using TechnicalTest.Api.Services.Interfaces;

namespace TechnicalTest.Api.Features.Products.Queries;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductResponse>>
{
    private readonly IProductService _productService;

    public GetAllProductsQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IEnumerable<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetAllAsync();
    }
}
