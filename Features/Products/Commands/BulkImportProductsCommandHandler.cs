using MediatR;
using TechnicalTest.Api.DTOs.ProductDTO.Response;
using TechnicalTest.Api.Services.Interfaces;
namespace TechnicalTest.Api.Features.Products.Commands;

public class BulkImportProductsCommandHandler : IRequestHandler<BulkImportProductsCommand, bool>
{
    private readonly IProductService _productService;
    private readonly ILogger<BulkImportProductsCommandHandler> _logger;

    public BulkImportProductsCommandHandler(IProductService productService, ILogger<BulkImportProductsCommandHandler> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task<bool> Handle(BulkImportProductsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await _productService.BulkImportProductsAsync(request.File);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product");
            throw;
        }
    }
}