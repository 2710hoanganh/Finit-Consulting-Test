using MediatR;
using TechnicalTest.Api.DTOs.ProductDTO.Response;

namespace TechnicalTest.Api.Features.Categories.Queries;

public record GetProductWithinCategoryQuery(
        int PageNumber = 1,
        int PageSize = 20,
        string? SearchTerm = null,
        int? CategoryId = null) : IRequest<(IEnumerable<ProductResponse> Items, int TotalCount)>;