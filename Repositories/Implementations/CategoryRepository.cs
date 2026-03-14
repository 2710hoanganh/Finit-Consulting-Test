using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechnicalTest.Api.Data;
using TechnicalTest.Api.Entities;
using TechnicalTest.Api.Repositories.Interfaces;

namespace TechnicalTest.Api.Repositories.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<Category> _dbSet;
    private readonly ILogger<CategoryRepository> _logger;

    public CategoryRepository(AppDbContext context, ILogger<CategoryRepository> logger)
    {
        _context = context;
        _dbSet = context.Set<Category>();
        _logger = logger;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        try
        {
            return await _dbSet.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category by id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        try
        {
            return await _dbSet.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all categories");
            throw;
        }
    }

    public async Task<IEnumerable<Category>> FindAsync(Expression<Func<Category, bool>> predicate)
    {
        try
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding categories");
            throw;
        }
    }

    public async Task<(IEnumerable<Category> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<Category, bool>>? predicate = null,
        int pageNumber = 1,
        int pageSize = 20,
        Expression<Func<Category, object>>? orderBy = null,
        bool ascending = true)
    {
        try
        {
            IQueryable<Category> query = _dbSet;

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
            _logger.LogError(ex, "Error getting paged categories");
            throw;
        }
    }

    public async Task<Category> AddAsync(Category entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding category");
            throw;
        }
    }

    public async Task UpdateAsync(Category entity)
    {
        try
        {
            _dbSet.Update(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            throw;
        }
    }

    public async Task DeleteAsync(Category entity)
    {
        try
        {
            _dbSet.Remove(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
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

    public async Task<Category?> GetCategoryWithProductsAsync(int id)
    {
        try
        {
            return await _dbSet
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category with products by id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        try
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active categories");
            throw;
        }
    }
}
