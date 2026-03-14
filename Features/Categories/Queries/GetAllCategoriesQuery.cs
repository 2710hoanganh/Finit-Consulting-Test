using MediatR;
using TechnicalTest.Api.DTOs.CategoryDTO.Response;

namespace TechnicalTest.Api.Features.Categories.Queries;

public record GetAllCategoriesQuery : IRequest<IEnumerable<CategoryResponse>>;
