using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechnicalTest.Api.DTOs.CategoryDTO.Request;
using TechnicalTest.Api.DTOs.CategoryDTO.Response;
using TechnicalTest.Api.Features.Categories.Commands;
using TechnicalTest.Api.Features.Categories.Queries;
using TechnicalTest.Api.Helpers;
using TechnicalTest.Api.DTOs.ProductDTO.Response;

namespace TechnicalTest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAllCategories()
    {
        var query = new GetAllCategoriesQuery();
        var categories = await _mediator.Send(query);
        return Ok(categories);
    }

    [HttpGet("{id}/products")]
    public async Task<ActionResult<PagedResult<ProductResponse>>> GetProductsByCategory(
        int id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetProductWithinCategoryQuery(pageNumber, pageSize, searchTerm, id);
        var (items, totalCount) = await _mediator.Send(query);

        var pagedResult = PaginationHelper.CreatePagedResult(items, totalCount, pageNumber, pageSize);
        return Ok(pagedResult);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<CategoryResponse>>> GetPagedCategories(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetPagedCategoriesQuery(pageNumber, pageSize);
        var (items, totalCount) = await _mediator.Send(query);

        var pagedResult = PaginationHelper.CreatePagedResult(items, totalCount, pageNumber, pageSize);
        return Ok(pagedResult);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponse>> GetCategory(int id)
    {
        var query = new GetCategoryByIdQuery(id);
        var category = await _mediator.Send(query);

        if (category == null)
            return NotFound();

        return Ok(category);
    }

    [HttpPost("create")]
    public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody] CategoryCreateRequest request)
    {
        var command = new CreateCategoryCommand(request);
        var category = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [HttpPut("update")]
    public async Task<ActionResult<CategoryResponse>> UpdateCategory([FromBody] CategoryUpdateRequest request)
    {
        var command = new UpdateCategoryCommand(request);
        var category = await _mediator.Send(command);
        return Ok(category);
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<bool>> DeleteCategory(int id)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound();

        return Ok(result);
    }
}
