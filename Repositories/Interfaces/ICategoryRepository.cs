using TechnicalTest.Api.Entities;

namespace TechnicalTest.Api.Repositories.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetCategoryWithProductsAsync(int id);
    Task<IEnumerable<Category>> GetActiveCategoriesAsync();
}
