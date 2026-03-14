using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechnicalTest.Api.DTOs.CategoryDTO.Request;
using TechnicalTest.Api.DTOs.ProductDTO.Request;
using TechnicalTest.Api.DTOs.ProductDTO.Response;
using TechnicalTest.Api.Features.Products.Commands;
using TechnicalTest.Api.Features.Products.Queries;
using TechnicalTest.Api.Helpers;
using TechnicalTest.Api.Infrastructure.MessageQueue;

namespace TechnicalTest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMessageQueueService _messageQueueService;
    private readonly ICsvParser<ProductImportDto> _csvParser;
    private const int BatchSize = 300;

    public ProductsController(
        IMediator mediator,
        IMessageQueueService messageQueueService,
        ICsvParser<ProductImportDto> csvParser)
    {
        _mediator = mediator;
        _messageQueueService = messageQueueService;
        _csvParser = csvParser;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAllProducts()
    {
        var query = new GetAllProductsQuery();
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<ProductResponse>>> GetPagedProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? categoryId = null)
    {
        var query = new GetPagedProductsQuery(pageNumber, pageSize, searchTerm, categoryId);
        var (items, totalCount) = await _mediator.Send(query);

        var pagedResult = PaginationHelper.CreatePagedResult(items, totalCount, pageNumber, pageSize);
        return Ok(pagedResult);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetProduct(int id)
    {
        var query = new GetProductByIdQuery(id);
        var product = await _mediator.Send(query);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost("create")]
    public async Task<ActionResult<ProductResponse>> CreateProduct([FromForm] ProductCreateRequest request)
    {
        var command = new CreateProductCommand(request);
        var product = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("update")]
    public async Task<ActionResult<ProductResponse>> UpdateProduct([FromForm] ProductUpdateRequest request)
    {
        var command = new UpdateProductCommand(request);
        var product = await _mediator.Send(command);
        return Ok(product);
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<bool>> DeleteProduct(int id)
    {
        var command = new DeleteProductCommand(id);
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> SearchProducts(
        [FromQuery] int? categoryId,
        [FromQuery] string? searchTerm,
        [FromQuery] DateTime? createFrom = null,
        [FromQuery] DateTime? createTo = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] decimal? priceFrom = null,
        [FromQuery] decimal? priceTo = null,
        [FromQuery] int? stockFrom = null,
        [FromQuery] int? stockTo = null)
    {
        var command = new SearchProductQuery(categoryId, searchTerm, createFrom, createTo, isActive, priceFrom, priceTo, stockFrom, stockTo);
        var products = await _mediator.Send(command);
        return Ok(products);
    }

    [HttpPost("import")]
    public async Task<ActionResult<ProductImportResult>> ImportProducts(IFormFile file)
    {
        var result = await _mediator.Send(new BulkImportProductsCommand(file));
        return Ok(result);
    }
}
