# TechnicalTest API

## Introduction

This is a RESTful API built on **.NET 8** using **CQRS** architecture with **MediatR**. The project manages products and categories, integrated with:

- **Database**: SQL Server with Entity Framework Core
- **Message Queue**: RabbitMQ
- **Object Storage**: MinIO (S3-compatible)
- **Logging**: Serilog
- **API Documentation**: Swagger

## Project Structure

```
TechnicalTest.Api/
├── Controllers/           # API Controllers
├── DTOs/                  # Data Transfer Objects
├── Entities/              # Database entities
├── Features/              # CQRS Handlers (Commands & Queries)
├── Helpers/               # Utilities (CSV Parser, Pagination)
├── Infrastructure/        # Background Services, Cache, MessageQueue
├── Mappings/              # AutoMapper profiles
├── Repositories/          # Data access layer
├── Services/              # Business logic layer
├── Seed/                  # Database seeding
└── Data/                  # DbContext, Migrations
```

## How to Run

### Requirements

- .NET 8 SDK
- SQL Server
- Docker (for Redis, RabbitMQ, MinIO)

### Run with Docker Compose

```bash
# Run all services (SQL Server, Redis, RabbitMQ, MinIO)
docker-compose up -d

# Run API
dotnet run --project TechnicalTest.Api/TechnicalTest.Api.csproj
```

### Configuration

The `appsettings.json` file contains the necessary configurations:

| Service | Description |
|---------|-------------|
| ConnectionStrings:DefaultConnection | SQL Server connection string |
| Redis:Host, Port, Password | Redis configuration |
| RabbitMQ:Host, Port, Username, Password | RabbitMQ configuration |
| Minio:Endpoint, AccessKey, SecretKey, BucketName | MinIO configuration |

### API Endpoints

After running, access Swagger UI at: `http://localhost:5000/swagger`

---

## API Reference

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products/paged` | Get products with pagination |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products/create` | Create new product |
| PUT | `/api/products/update` | Update product |
| DELETE | `/api/products/delete/{id}` | Delete product |
| GET | `/api/products/search` | Search products with filters |
| POST | `/api/products/import` | Bulk import products from CSV |

#### GET /api/products

Get all products.

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Product Name",
    "description": "Description",
    "price": 100.00,
    "stock": 50,
    "categoryId": 1,
    "categoryName": "Category Name",
    "imageUrl": "https://...",
    "isActive": true,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
]
```

#### GET /api/products/paged

Get products with pagination.

**Query Parameters:**
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 20 | Items per page |
| searchTerm | string | null | Search by name |
| categoryId | int | null | Filter by category |

**Response:** `200 OK`
```json
{
  "items": [],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 100,
  "totalPages": 5
}
```

#### GET /api/products/{id}

Get product by ID.

**Response:** `200 OK` or `404 Not Found`

#### POST /api/products/create

Create new product.

**Request (multipart/form-data):**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| Name | string | Yes | Product name |
| Description | string | No | Product description |
| Price | decimal | Yes | Product price |
| Stock | int | Yes | Stock quantity |
| CategoryId | int | Yes | Category ID |
| Image | IFormFile | No | Product image |

**Response:** `201 Created`

#### PUT /api/products/update

Update product.

**Request (multipart/form-data):**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| Id | int | Yes | Product ID |
| Name | string | Yes | Product name |
| Description | string | No | Product description |
| Price | decimal | Yes | Product price |
| Stock | int | Yes | Stock quantity |
| CategoryId | int | Yes | Category ID |
| Image | IFormFile | No | Product image |

**Response:** `200 OK`

#### DELETE /api/products/delete/{id}

Delete product.

**Response:** `200 OK` or `404 Not Found`

#### GET /api/products/search

Search products with advanced filters.

**Query Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| categoryId | int? | Filter by category |
| searchTerm | string? | Search in name/description |
| createFrom | DateTime? | Created date from |
| createTo | DateTime? | Created date to |
| isActive | bool? | Filter by active status |
| priceFrom | decimal? | Price from |
| priceTo | decimal? | Price to |
| stockFrom | int? | Stock from |
| stockTo | int? | Stock to |

**Response:** `200 OK`

#### POST /api/products/import

Bulk import products from CSV file.

**Request (multipart/form-data):**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| file | IFormFile | Yes | CSV file |

**CSV Format:**
```csv
Name,Description,Price,Stock,CategoryId
Product1,Description1,100.00,50,1
Product2,Description2,200.00,30,2
```

**Response:** `200 OK`
```json
{
  "success": true,
  "message": "File uploaded successfully. Processing in background."
}
```

---

### Categories

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/categories` | Get all categories |
| GET | `/api/categories/paged` | Get categories with pagination |
| GET | `/api/categories/{id}` | Get category by ID |
| GET | `/api/categories/{id}/products` | Get products in category |
| POST | `/api/categories/create` | Create new category |
| PUT | `/api/categories/update` | Update category |
| DELETE | `/api/categories/delete/{id}` | Delete category |

#### GET /api/categories

Get all categories.

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Category Name",
    "parentId": null,
    "isActive": true,
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

#### GET /api/categories/paged

Get categories with pagination.

**Query Parameters:**
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 20 | Items per page |

**Response:** `200 OK`

#### GET /api/categories/{id}

Get category by ID.

**Response:** `200 OK` or `404 Not Found`

#### GET /api/categories/{id}/products

Get products within a category.

**Query Parameters:**
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 20 | Items per page |
| searchTerm | string | null | Search products |

**Response:** `200 OK`

#### POST /api/categories/create

Create new category.

**Request (application/json):**
```json
{
  "name": "Category Name",
  "parentId": null
}
```

**Response:** `201 Created`

#### PUT /api/categories/update

Update category.

**Request (application/json):**
```json
{
  "id": 1,
  "name": "Updated Name",
  "parentId": null,
  "isActive": true
}
```

**Response:** `200 OK`

#### DELETE /api/categories/delete/{id}

Delete category.

**Response:** `200 OK` or `404 Not Found`

---

## Bulk Insert Solution

### 1. Bulk Import Flow

The project uses **Asynchronous Processing** architecture with Message Queue:

```
Client Upload CSV
       │
       ▼
  API receives file
       │
       ▼
  Parse CSV → Publish to RabbitMQ (batched)
       │
       ▼
  ProductImportWorker (Background Service)
       │
       ▼
  Validate each product
       │
       ▼
  SqlBulkCopy to Database (fast!)
       │
       ▼
  Update Cache (optional)
```

### 2. Implementation Details

#### Step 1: Upload file and parse CSV

See [ProductsController.cs](TechnicalTest.Api/Controllers/ProductsController.cs) - POST `/api/products/import`:

```csharp
[HttpPost("import")]
public async Task<ActionResult<ProductImportResult>> ImportProducts(IFormFile file)
{
    var products = _csvParser.Parse(file.OpenReadStream()).ToList();

    // Split into batches of 300
    var batches = products
        .Select((product, index) => new { product, index })
        .GroupBy(x => x.index / BatchSize)
        .Select(g => g.Select(x => x.product).ToList())
        .ToList();

    // Publish each batch to queue
    for (int i = 0; i < batches.Count; i++)
    {
        var message = new ProductImportMessage
        {
            Products = batches[i],
            BatchNumber = i + 1,
            TotalBatches = batches.Count
        };
        _messageQueueService.Publish(_queueName, message);
    }

    return Ok(new ProductImportResult { Success = true });
}
```

#### Step 2: Background Worker - Process batch

See [ProductImportWorker.cs](TechnicalTest.Api/Infrastructure/BackgroundServices/ProductImportWorker.cs):

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    // Connect to RabbitMQ
    var factory = new ConnectionFactory
    {
        HostName = _configuration["RabbitMQ:Host"],
        Port = int.Parse(_configuration["RabbitMQ:Port"]),
        UserName = _configuration["RabbitMQ:UserName"],
        Password = _configuration["RabbitMQ:Password"]
    };

    using var connection = factory.CreateConnection();
    using var channel = connection.CreateModel();

    channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += async (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);
        var message = JsonSerializer.Deserialize<ProductImportMessage>(json);

        if (message != null)
        {
            await ProcessBatchAsync(message);
        }
        channel.BasicAck(ea.DeliveryTag, multiple: false);
    };

    channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

    while (!stoppingToken.IsCancellationRequested)
    {
        await Task.Delay(1000, stoppingToken);
    }
}
```

#### Step 3: Process Batch with Validation and SqlBulkCopy

See [ProductImportWorker.cs](TechnicalTest.Api/Infrastructure/BackgroundServices/ProductImportWorker.cs):

```csharp
private async Task ProcessBatchAsync(ProductImportMessage message)
{
    var productsToAdd = new List<Product>();
    var errors = new List<string>();

    // Validate each product
    foreach (var dto in message.Products)
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
            SKU = dto.SKU ?? $"SKU-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
            ImageUrl = dto.Image,
            CategoryId = dto.CategoryId,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt ?? DateTime.UtcNow
        });
    }

    // Use SqlBulkCopy for fast insert
    await BulkInsertProductsAsync(productsToAdd);

    _logger.LogInformation(
        "Processed batch {Batch}/{Total}. Total: {Total}, Errors: {Errors}",
        message.BatchNumber,
        message.TotalBatches,
        productsToAdd.Count,
        errors.Count);
}

// SqlBulkCopy - Much faster than EF Core
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

    // Map all columns
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

// Convert Product list to DataTable for SqlBulkCopy
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
```

### 3. Bulk Insert Optimizations

| Technique | Description |
|-----------|-------------|
| **Batch Processing** | Split products into batches for RabbitMQ messages |
| **SqlBulkCopy** | Use ADO.NET SqlBulkCopy instead of EF Core - **10-100x faster** |
| **BatchSize = 5000** | Insert 5000 rows per batch for optimal performance |
| **No ChangeTracker** | SqlBulkCopy bypasses EF Core completely - no tracking overhead |
| **Validation** | Validate each product before insert, skip invalid ones |
| **Error Tracking** | Collect and log errors without stopping the batch |
| **Prefetch Count** | Set prefetchCount=1 to process one message at a time |
| **Durable Queue** | Queue declared as durable for message persistence |

### 4. Message Structure

```csharp
public class ProductImportMessage
{
    public List<ProductImportDto> Products { get; set; }
    public int BatchNumber { get; set; }
    public int TotalBatches { get; set; }
}

public class ProductImportDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int Stock { get; set; }
    public string SKU { get; set; }
    public string Image { get; set; }
    public int CategoryId { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
}
```

### 5. Results

- API returns immediately after publishing messages to queue
- Background worker processes batches asynchronously
- Invalid products are logged but don't stop the batch
- Can scale horizontally by running multiple instances
- Each batch is processed independently with error handling
