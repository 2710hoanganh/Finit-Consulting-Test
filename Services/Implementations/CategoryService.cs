using AutoMapper;
using TechnicalTest.Api.DTOs.CategoryDTO.Request;
using TechnicalTest.Api.DTOs.CategoryDTO.Response;
using TechnicalTest.Api.Entities;
using TechnicalTest.Api.Repositories.Interfaces;
using TechnicalTest.Api.Services.Interfaces;

namespace TechnicalTest.Api.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetActiveCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all categories");
            throw;
        }
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetCategoryWithProductsAsync(id);
            return category == null ? null : _mapper.Map<CategoryResponse>(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category by id {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return false;

            await _categoryRepository.DeleteAsync(category);
            await _categoryRepository.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {Id}", id);
            throw;
        }
    }

    public async Task<(IEnumerable<CategoryResponse> Items, int TotalCount)> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 20)
    {
        try
        {
            var (items, totalCount) = await _categoryRepository.GetPagedAsync(
                predicate: c => c.IsActive,
                pageNumber: pageNumber,
                pageSize: pageSize,
                orderBy: c => c.Name,
                ascending: true);

            return (_mapper.Map<IEnumerable<CategoryResponse>>(items), totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged categories");
            throw;
        }
    }

    public async Task<CategoryResponse> CreateAsync(CategoryCreateRequest request)
    {
        try
        {
            var category = _mapper.Map<Category>(request);
            category.CreatedAt = DateTime.UtcNow;

            var result = await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return _mapper.Map<CategoryResponse>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            throw;
        }
    }

    public async Task<CategoryResponse> UpdateAsync(CategoryUpdateRequest request)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id);
            if (category == null)
                throw new KeyNotFoundException($"Category with id {request.Id} not found");

            // Only update fields that are provided (not null)
            if (!string.IsNullOrEmpty(request.Name))
                category.Name = request.Name;

            if (request.Description != null)
                category.Description = request.Description;

            if (request.ParentCategoryId.HasValue)
                category.ParentCategoryId = request.ParentCategoryId;

            if (request.SortOrder.HasValue)
                category.SortOrder = request.SortOrder.Value;

            if (request.IsActive.HasValue)
                category.IsActive = request.IsActive.Value;

            category.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return _mapper.Map<CategoryResponse>(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {Id}", request.Id);
            throw;
        }
    }
}
