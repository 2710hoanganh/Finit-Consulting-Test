using System.Data;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TechnicalTest.Api.Data;
using TechnicalTest.Api.Entities;
using TechnicalTest.Api.Infrastructure.MessageQueue;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TechnicalTest.Api.Infrastructure.BackgroundServices;

public class ProductImportWorker : BackgroundService
{
    private readonly ILogger<ProductImportWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly string _queueName = "product-import-queue";

    public ProductImportWorker(
        ILogger<ProductImportWorker> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Product Import Worker started");

        await Task.Delay(5000, stoppingToken);

        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]),
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };

        try
        {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                try
                {
                    var message = JsonSerializer.Deserialize<ProductImportMessage>(json);
                    if (message != null)
                    {
                        await ProcessBatchAsync(message);
                    }
                    channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing product import batch");
                    channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Product Import Worker");
        }
    }

    private async Task ProcessBatchAsync(ProductImportMessage message)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var productsToAdd = new List<Product>();
        var errors = new List<string>();

        try
        {
            foreach (var dto in message.Products)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(dto.Name))
                    {
                        errors.Add("Row with empty name skipped");
                        continue;
                    }

                    if (dto.Price <= 0)
                    {
                        errors.Add($"Product '{dto.Name}' has invalid price");
                        continue;
                    }

                    productsToAdd.Add(new Product
                    {
                        Name = dto.Name,
                        Description = dto.Description,
                        Price = dto.Price,
                        DiscountPrice = dto.DiscountPrice,
                        Stock = dto.Stock,
                        SKU = dto.SKU + Guid.NewGuid().ToString() ?? $"SKU-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
                        ImageUrl = dto.Image,
                        CategoryId = dto.CategoryId,
                        IsActive = dto.IsActive,
                        CreatedAt = dto.CreatedAt ?? DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    errors.Add($"Error processing product '{dto.Name}': {ex.Message}");
                }
            }

            // Use SqlBulkCopy for much faster insert
            await BulkInsertProductsAsync(productsToAdd);

            _logger.LogInformation(
                "Processed batch {Batch}/{Total}. Total: {Total}, Errors: {Errors}",
                message.BatchNumber,
                message.TotalBatches,
                productsToAdd.Count,
                errors.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing batch {Batch}", message.BatchNumber);
            throw;
        }
    }

    private async Task BulkInsertProductsAsync(List<Product> products)
    {
        if (!products.Any()) return;

        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var bulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = "Products",
            BatchSize = 5000,
            BulkCopyTimeout = 300
        };

        // Map columns
        bulkCopy.ColumnMappings.Add(nameof(Product.Name), "Name");
        bulkCopy.ColumnMappings.Add(nameof(Product.Description), "Description");
        bulkCopy.ColumnMappings.Add(nameof(Product.Price), "Price");
        bulkCopy.ColumnMappings.Add(nameof(Product.DiscountPrice), "DiscountPrice");
        bulkCopy.ColumnMappings.Add(nameof(Product.Stock), "Stock");
        bulkCopy.ColumnMappings.Add(nameof(Product.SKU), "SKU");
        bulkCopy.ColumnMappings.Add(nameof(Product.ImageUrl), "ImageUrl");
        bulkCopy.ColumnMappings.Add(nameof(Product.CategoryId), "CategoryId");
        bulkCopy.ColumnMappings.Add(nameof(Product.CustomAttributes), "CustomAttributes");
        bulkCopy.ColumnMappings.Add(nameof(Product.CreatedAt), "CreatedAt");
        bulkCopy.ColumnMappings.Add(nameof(Product.UpdatedAt), "UpdatedAt");
        bulkCopy.ColumnMappings.Add(nameof(Product.IsActive), "IsActive");

        var dataTable = ConvertToDataTable(products);
        await bulkCopy.WriteToServerAsync(dataTable);

        _logger.LogInformation("SqlBulkCopy completed for {Count} products", products.Count);
    }

    private DataTable ConvertToDataTable(List<Product> products)
    {
        var dt = new DataTable();
        dt.Columns.Add("Id", typeof(int));
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Description", typeof(string));
        dt.Columns.Add("Price", typeof(decimal));
        dt.Columns.Add("DiscountPrice", typeof(decimal));
        dt.Columns.Add("Stock", typeof(int));
        dt.Columns.Add("SKU", typeof(string));
        dt.Columns.Add("ImageUrl", typeof(string));
        dt.Columns.Add("CategoryId", typeof(int));
        dt.Columns.Add("CustomAttributes", typeof(string));
        dt.Columns.Add("CreatedAt", typeof(DateTime));
        dt.Columns.Add("UpdatedAt", typeof(DateTime));
        dt.Columns.Add("IsActive", typeof(bool));
        dt.Columns.Add("RowVersion", typeof(byte[]));

        foreach (var product in products)
        {
            dt.Rows.Add(
                0, // Id will be auto-generated by DB
                product.Name,
                product.Description ?? (object)DBNull.Value,
                product.Price,
                product.DiscountPrice ?? (object)DBNull.Value,
                product.Stock,
                product.SKU ?? (object)DBNull.Value,
                product.ImageUrl ?? (object)DBNull.Value,
                product.CategoryId,
                product.CustomAttributes != null ? JsonSerializer.Serialize(product.CustomAttributes) : (object)DBNull.Value,
                product.CreatedAt,
                product.UpdatedAt ?? (object)DBNull.Value,
                product.IsActive,
                (object)DBNull.Value
            );
        }

        return dt;
    }
}
