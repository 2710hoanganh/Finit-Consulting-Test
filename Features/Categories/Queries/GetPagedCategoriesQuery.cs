using MediatR;
using TechnicalTest.Api.DTOs.CategoryDTO.Response;

namespace TechnicalTest.Api.Features.Categories.Queries;

public record GetPagedCategoriesQuery(
    int PageNumber = 1,
    int PageSize = 20) : IRequest<(IEnumerable<CategoryResponse> Items, int TotalCount)>;
