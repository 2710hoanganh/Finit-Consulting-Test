using MediatR;
using TechnicalTest.Api.DTOs.ProductDTO.Request;
using TechnicalTest.Api.DTOs.ProductDTO.Response;

namespace TechnicalTest.Api.Features.Products.Commands;

public record CreateProductCommand(ProductCreateRequest Request) : IRequest<ProductResponse>;
