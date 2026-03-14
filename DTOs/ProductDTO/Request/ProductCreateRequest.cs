using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TechnicalTest.Api.DTOs.ProductDTO.Request;

public class ProductCreateRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
    public decimal Price { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "DiscountPrice must be greater than or equal to 0")]
    public decimal? DiscountPrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to 0")]
    public int Stock { get; set; }

    [MaxLength(100, ErrorMessage = "SKU cannot exceed 100 characters")]
    public string? SKU { get; set; }
    [Required]
    public IFormFile? Image { get; set; }
    [Required(ErrorMessage = "CategoryId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than 0")]
    public int CategoryId { get; set; }
}
