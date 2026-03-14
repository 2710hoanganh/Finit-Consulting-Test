using MediatR;
using TechnicalTest.Api.DTOs.ProductDTO.Response;

namespace TechnicalTest.Api.Features.Products.Queries;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductResponse>>;
