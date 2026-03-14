using MediatR;
namespace TechnicalTest.Api.Features.Products.Commands;

public record BulkImportProductsCommand(IFormFile File) : IRequest<bool>;