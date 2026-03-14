using System.ComponentModel.DataAnnotations;

namespace TechnicalTest.Api.DTOs.CategoryDTO.Request;

public class CategoryUpdateRequest
{
    [Required(ErrorMessage = "Id is required")]
    public int Id { get; set; }

    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string? Name { get; set; }

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsActive { get; set; }
}
