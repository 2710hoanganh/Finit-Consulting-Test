using MediatR;
using TechnicalTest.Api.DTOs.ProductDTO.Response;

namespace TechnicalTest.Api.Features.Products.Queries;

public record GetPagedProductsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    int? CategoryId = null) : IRequest<(IEnumerable<ProductResponse> Items, int TotalCount)>;
