using AutoMapper;
using TechnicalTest.Api.DTOs.ProductDTO.Request;
using TechnicalTest.Api.DTOs.ProductDTO.Response;
using TechnicalTest.Api.Entities;
using TechnicalTest.Api.Repositories.Interfaces;
using TechnicalTest.Api.Services.Interfaces;
using TechnicalTest.Api.Helpers;
using TechnicalTest.Api.Infrastructure.MessageQueue;

namespace TechnicalTest.Api.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;
    private readonly IS3Service _s3Service;
    private readonly ICsvParser<ProductImportDto> _csvParser;
    private readonly IMessageQueueService _messageQueueService;

    public ProductService(IProductRepository productRepository,
    IMapper mapper,
    ILogger<ProductService> logger, IS3Service s3Service, IMessageQueueService messageQueueService, ICsvParser<ProductImportDto> csvParser)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
        _s3Service = s3Service;
        _messageQueueService = messageQueueService;
        _csvParser = csvParser;
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        try
        {
            var products = await _productRepository.GetProductsWithCategoriesAsync();
            return _mapper.Map<IEnumerable<ProductResponse>>(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all products");
            throw;
        }
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetProductWithCategoryAsync(id);
            return product == null ? null : _mapper.Map<ProductResponse>(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product by id {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            await _productRepository.DeleteAsync(product);
            await _productRepository.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {Id}", id);
            throw;
        }
    }

    public async Task<(IEnumerable<ProductResponse> Items, int TotalCount)> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 20)
    {
        try
        {
            var (items, totalCount) = await _productRepository.GetPagedWithCategoryAsync(
                pageNumber, pageSize, null, null);

            return (_mapper.Map<IEnumerable<ProductResponse>>(items), totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged products");
            throw;
        }
    }

    public async Task<ProductResponse> CreateAsync(ProductCreateRequest request)
    {
        try
        {
            var product = _mapper.Map<Product>(request);
            product.CreatedAt = DateTime.UtcNow;
            product.IsActive = true;

            if (request.Image != null)
            {
                using var imageStream = request.Image.OpenReadStream();
                var fileKey = await _s3Service.UploadFileAsync(imageStream, request.Image.FileName, request.Image.ContentType);
                product.ImageUrl = _s3Service.GetUri(fileKey);
            }
            else
            {
                _logger.LogWarning("No image provided for product creation");
            }

            var result = await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            var createdProduct = await _productRepository.GetProductWithCategoryAsync(result.Id);
            return _mapper.Map<ProductResponse>(createdProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            throw;
        }
    }

    public async Task<ProductResponse> UpdateAsync(ProductUpdateRequest request)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
                throw new KeyNotFoundException($"Product with id {request.Id} not found");

            if (!string.IsNullOrEmpty(request.Name))
                product.Name = request.Name;

            if (request.Description != null)
                product.Description = request.Description;

            if (request.Price.HasValue)
                product.Price = request.Price.Value;

            if (request.DiscountPrice.HasValue)
                product.DiscountPrice = request.DiscountPrice;

            if (request.Stock.HasValue)
                product.Stock = request.Stock.Value;

            if (request.SKU != null)
                product.SKU = request.SKU;

            if (request.CategoryId.HasValue)
                product.CategoryId = request.CategoryId.Value;

            if (request.IsActive.HasValue)
                product.IsActive = request.IsActive.Value;

            if (request.ImageUrl != null)
            {
                using var imageStream = request.ImageUrl.OpenReadStream();
                var fileKey = await _s3Service.UploadFileAsync(imageStream, request.ImageUrl.FileName, request.ImageUrl.ContentType);
                product.ImageUrl = _s3Service.GetUri(fileKey);
            }

            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            var updatedProduct = await _productRepository.GetProductWithCategoryAsync(product.Id);
            return _mapper.Map<ProductResponse>(updatedProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {Id}", request.Id);
            throw;
        }
    }

    public async Task<(IEnumerable<ProductResponse> Items, int TotalCount)> GetPagedProductsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        int? categoryId = null)
    {
        try
        {
            var (items, totalCount) = await _productRepository.GetPagedWithCategoryAsync(
                pageNumber, pageSize, searchTerm, categoryId);

            return (_mapper.Map<IEnumerable<ProductResponse>>(items), totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged products with filters");
            throw;
        }
    }

    public async Task<(IEnumerable<ProductResponse> Items, int TotalCount)> GetPagedWithCategoryAsync(
        int pageNumber,
        int pageSize,
        string searchTerm,
        int categoryId)
    {
        try
        {
            var (items, totalCount) = await _productRepository.GetPagedWithCategoryAsync(
                pageNumber, pageSize, searchTerm, categoryId);

            return (_mapper.Map<IEnumerable<ProductResponse>>(items), totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged products with category");
            throw;
        }
    }

    public async Task<(IEnumerable<ProductResponse> Items, int TotalCount)> SearchProductsAsync(int categoryId, string searchTerm, DateTime? createFrom = null, DateTime? createTo = null, bool? isActive = null, decimal? priceFrom = null, decimal? priceTo = null, int? stockFrom = null, int? stockTo = null)
    {
        try
        {
            var (items, totalCount) = await _productRepository.SearchProduct(categoryId, searchTerm, createFrom, createTo, isActive, priceFrom, priceTo, stockFrom, stockTo);
            return (_mapper.Map<IEnumerable<ProductResponse>>(items), totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products");
            throw;
        }
    }

    public async Task<(IEnumerable<ProductResponse> Items, int TotalCount)> SearchProductAsync(
        int? categoryId,
        string? searchTerm,
        DateTime? createFrom = null,
        DateTime? createTo = null,
        bool? isActive = null,
        decimal? priceFrom = null,
        decimal? priceTo = null,
        int? stockFrom = null,
        int? stockTo = null)
    {
        try
        {
            var (items, totalCount) = await _productRepository.SearchProduct(categoryId, searchTerm, createFrom, createTo, isActive, priceFrom, priceTo, stockFrom, stockTo);
            return (_mapper.Map<IEnumerable<ProductResponse>>(items), totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products");
            throw;
        }
    }

    public async Task<bool> BulkImportProductsAsync(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file provided for bulk import");
                return false;
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Invalid file format for bulk import: {FileName}", file.FileName);
                return false;
            }

            using var stream = file.OpenReadStream();
            var products = _csvParser.Parse(stream).ToList();

            if (!products.Any())
            {
                _logger.LogWarning("No data found in CSV file: {FileName}", file.FileName);
                return false;
            }

            var batches = products
                .Select((product, index) => new { product, index })
                .GroupBy(x => x.index / 5000) //batch size of 2000
                .Select(g => g.Select(x => x.product).ToList())
                .ToList();

            var jobId = Guid.NewGuid().ToString();
            int totalBatches = batches.Count;

            for (int i = 0; i < batches.Count; i++)
            {
                var message = new ProductImportMessage
                {
                    JobId = jobId,
                    Products = batches[i],
                    BatchNumber = i + 1,
                    TotalBatches = totalBatches
                };

                await _messageQueueService.PublishAsync(message, "product-import-queue");
                _logger.LogInformation("Published batch {BatchNumber}/{TotalBatches} for job {JobId}", i + 1, totalBatches, jobId);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk import of products");
            return false;
        }
    }
}
