using MediatR;
using TechnicalTest.Api.DTOs.CategoryDTO.Request;
using TechnicalTest.Api.DTOs.CategoryDTO.Response;

namespace TechnicalTest.Api.Features.Categories.Commands;

public record CreateCategoryCommand(CategoryCreateRequest Request) : IRequest<CategoryResponse>;
