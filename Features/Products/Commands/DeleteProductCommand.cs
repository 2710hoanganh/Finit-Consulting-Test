using MediatR;

namespace TechnicalTest.Api.Features.Products.Commands;

public record DeleteProductCommand(int Id) : IRequest<bool>;
