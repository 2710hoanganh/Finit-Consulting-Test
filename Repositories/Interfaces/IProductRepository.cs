using TechnicalTest.Api.Entities;

namespace TechnicalTest.Api.Repositories.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetProductWithCategoryAsync(int id);
    Task<IEnumerable<Product>> GetProductsWithCategoriesAsync();
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedWithCategoryAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        int? categoryId = null);
    
    Task<(IEnumerable<Product> Items, int TotalCount)> SearchProduct (int? categoryId,
    string? searchTerm,
    DateTime? createFrom = null,
    DateTime? createTo = null,
    bool? isActive = null,
    decimal? priceFrom = null,
    decimal? priceTo = null,
    int? stockFrom = null,
    int? stockTo = null);
}
