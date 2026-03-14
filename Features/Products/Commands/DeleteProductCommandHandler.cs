using MediatR;
using TechnicalTest.Api.Services.Interfaces;

namespace TechnicalTest.Api.Features.Products.Commands;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductService _productService;

    public DeleteProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.DeleteAsync(request.Id);
    }
}
