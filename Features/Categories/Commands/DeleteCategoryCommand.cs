using MediatR;

namespace TechnicalTest.Api.Features.Categories.Commands;

public record DeleteCategoryCommand(int Id) : IRequest<bool>;
