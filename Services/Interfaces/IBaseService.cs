namespace TechnicalTest.Api.Services.Interfaces;

public interface IBaseService<TResponse, TCreateRequest, TUpdateRequest>
    where TResponse : class
    where TCreateRequest : class
    where TUpdateRequest : class
{
    Task<IEnumerable<TResponse>> GetAllAsync();
    Task<TResponse?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<(IEnumerable<TResponse> Items, int TotalCount)> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 20);
    Task<TResponse> CreateAsync(TCreateRequest request);
    Task<TResponse> UpdateAsync(TUpdateRequest request);
}
