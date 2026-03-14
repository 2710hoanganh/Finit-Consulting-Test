using TechnicalTest.Api.DTOs.ProductDTO.Request;
using TechnicalTest.Api.DTOs.ProductDTO.Response;

namespace TechnicalTest.Api.Services.Interfaces;

public interface IProductService : IBaseService<ProductResponse, ProductCreateRequest, ProductUpdateRequest>
{
    Task<(IEnumerable<ProductResponse> Items, int TotalCount)> GetPagedProductsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        int? categoryId = null);

    Task<(IEnumerable<ProductResponse> Items, int TotalCount)> GetPagedWithCategoryAsync(
        int pageNumber,
        int pageSize,
        string searchTerm,
        int categoryId);

    Task<(IEnumerable<ProductResponse> Items, int TotalCount)> SearchProductAsync(
        int? categoryId,
        string? searchTerm,
        DateTime? createFrom = null,
        DateTime? createTo = null,
        bool? isActive = null,
        decimal? priceFrom = null,
        decimal? priceTo = null,
        int? stockFrom = null,
        int? stockTo = null);

    Task<bool> BulkImportProductsAsync(IFormFile file);
}
