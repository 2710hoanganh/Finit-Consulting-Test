using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace TechnicalTest.Api.DTOs.ProductDTO.Request;

public class ProductImportDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    public decimal Price { get; set; }

    public decimal? DiscountPrice { get; set; }

    public int Stock { get; set; }

    [MaxLength(100, ErrorMessage = "SKU cannot exceed 100 characters")]
    public string? SKU { get; set; }

    public string? Image { get; set; }

    [Required(ErrorMessage = "CategoryId is required")]
    public int CategoryId { get; set; }
    [Ignore]
    public string? CustomAtt { get; set; }
    [Ignore]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    [Ignore]
    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;
}

public class ProductImportResult
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public string? JobId { get; set; }
}
