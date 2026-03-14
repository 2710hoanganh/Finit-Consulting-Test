using System.ComponentModel.DataAnnotations;

namespace TechnicalTest.Api.DTOs.ProductDTO.Request;

public class ProductUpdateRequest
{
    [Required(ErrorMessage = "Id is required")]
    public int Id { get; set; }

    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string? Name { get; set; }

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
    public decimal? Price { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "DiscountPrice must be greater than or equal to 0")]
    public decimal? DiscountPrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to 0")]
    public int? Stock { get; set; }

    [MaxLength(100, ErrorMessage = "SKU cannot exceed 100 characters")]
    public string? SKU { get; set; }

    public IFormFile? ImageUrl { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than 0")]
    public int? CategoryId { get; set; }

    public bool? IsActive { get; set; }
}
