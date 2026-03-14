using MediatR;
using TechnicalTest.Api.DTOs.ProductDTO.Response;
using TechnicalTest.Api.Services.Interfaces;

namespace TechnicalTest.Api.Features.Products.Commands;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    private readonly IProductService _productService;

    public UpdateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.UpdateAsync(request.Request);
    }
}
