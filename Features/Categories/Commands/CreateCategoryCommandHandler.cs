using MediatR;
using TechnicalTest.Api.DTOs.CategoryDTO.Response;
using TechnicalTest.Api.Services.Interfaces;

namespace TechnicalTest.Api.Features.Categories.Commands;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    private readonly ICategoryService _categoryService;

    public CreateCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        return await _categoryService.CreateAsync(request.Request);
    }
}
