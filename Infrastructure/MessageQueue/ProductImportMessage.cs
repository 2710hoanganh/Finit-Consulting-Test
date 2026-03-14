using TechnicalTest.Api.DTOs.ProductDTO.Request;

namespace TechnicalTest.Api.Infrastructure.MessageQueue;

public class ProductImportMessage
{
    public string JobId { get; set; } = string.Empty;
    public List<ProductImportDto> Products { get; set; } = new();
    public int BatchNumber { get; set; }
    public int TotalBatches { get; set; }
}

public class ProductImportStatus
{
    public string JobId { get; set; } = string.Empty;
    public int TotalRows { get; set; }
    public int ProcessedRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed
    public List<string> Errors { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
