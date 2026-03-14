using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechnicalTest.Api.Data;
using TechnicalTest.Api.Entities;
using TechnicalTest.Api.Repositories.Interfaces;

namespace TechnicalTest.Api.Repositories.Implementations;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<Product> _dbSet;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(AppDbContext context, ILogger<ProductRepository> logger)
    {
        _context = context;
        _dbSet = context.Set<Product>();
        _logger = logger;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        try
        {
            return await _dbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product by id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            return await _dbSet
                .Include(p => p.Category)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all products");
            throw;
        }
    }

    public async Task<IEnumerable<Product>> FindAsync(Expression<Func<Product, bool>> predicate)
    {
        try
        {
            return await _dbSet
                .Include(p => p.Category)
                .Where(predicate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding products");
            throw;
        }
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<Product, bool>>? predicate = null,
        int pageNumber = 1,
        int pageSize = 20,
        Expression<Func<Product, object>>? orderBy = null,
        bool ascending = true)
    {
        try
        {
            IQueryable<Product> query = _dbSet.Include(p => p.Category);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = ascending
                    ? query.OrderBy(orderBy)
                    : query.OrderByDescending(orderBy);
            }

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged products");
            throw;
        }
    }

    public async Task<Product> AddAsync(Product entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product");
            throw;
        }
    }

    public async Task UpdateAsync(Product entity)
    {
        try
        {
            _dbSet.Update(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product");
            throw;
        }
    }

    public async Task DeleteAsync(Product entity)
    {
        try
        {
            _dbSet.Remove(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product");
            throw;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes");
            throw;
        }
    }

    public async Task<Product?> GetProductWithCategoryAsync(int id)
    {
        try
        {
            return await _dbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product with category by id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Product>> GetProductsWithCategoriesAsync()
    {
        try
        {
            return await _dbSet
                .Include(p => p.Category)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products with categories");
            throw;
        }
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedWithCategoryAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        int? categoryId = null)
    {
        try
        {
            IQueryable<Product> query = _dbSet.Include(p => p.Category);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                    (p.SKU != null && p.SKU.ToLower().Contains(searchTerm)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged products with category");
            throw;
        }
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> SearchProduct(int? categoryId, string? searchTerm, DateTime? createFrom = null, DateTime? createTo = null, bool? isActive = null, decimal? priceFrom = null, decimal? priceTo = null, int? stockFrom = null, int? stockTo = null)
    {
        try
        {
            IQueryable<Product> query = _dbSet.Where(p => p.CategoryId != null || p.CategoryId == categoryId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                    (p.SKU != null && p.SKU.ToLower().Contains(searchTerm)));
            }

            if (createFrom.HasValue)
                query = query.Where(p => p.CreatedAt >= createFrom.Value);

            if (createTo.HasValue)
                query = query.Where(p => p.CreatedAt <= createTo.Value);

            if (isActive.HasValue)
                query = query.Where(p => p.IsActive == isActive.Value);

            if (priceFrom.HasValue)
                query = query.Where(p => p.Price >= priceFrom.Value);

            if (priceTo.HasValue)
                query = query.Where(p => p.Price <= priceTo.Value);

            if (stockFrom.HasValue)
                query = query.Where(p => p.Stock >= stockFrom.Value);

            if (stockTo.HasValue)
                query = query.Where(p => p.Stock <= stockTo.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .Include(p => p.Category)
                .ToListAsync();

            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products");
            throw;
        }
    }
}
