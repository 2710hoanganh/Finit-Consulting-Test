using MediatR;
using TechnicalTest.Api.DTOs.ProductDTO.Response;

namespace TechnicalTest.Api.DTOs.CategoryDTO.Request;

public record SearchProductQuery(int? CategoryId, 
            string? SearchTerm, 
            DateTime? CreateFrom = null, 
            DateTime? CreateTo = null, 
            bool? IsActive = null, 
            decimal? PriceFrom = null, 
            decimal? PriceTo = null, 
            int? StockFrom = null, 
            int? StockTo = null) : IRequest<IEnumerable<ProductResponse>>;