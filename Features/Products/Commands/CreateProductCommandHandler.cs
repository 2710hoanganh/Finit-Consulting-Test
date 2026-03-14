using MediatR;
using TechnicalTest.Api.DTOs.ProductDTO.Response;
using TechnicalTest.Api.Services.Interfaces;

namespace TechnicalTest.Api.Features.Products.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponse>
{
    private readonly IProductService _productService;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IProductService productService, ILogger<CreateProductCommandHandler> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await _productService.CreateAsync(request.Request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product");
            throw;
        }
    }
}
