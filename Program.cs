using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;
using TechnicalTest.Api.Data;
using TechnicalTest.Api.DTOs.ProductDTO.Request;
using TechnicalTest.Api.Helpers;
using TechnicalTest.Api.Infrastructure.BackgroundServices;
using TechnicalTest.Api.Infrastructure.Cache;
using TechnicalTest.Api.Infrastructure.MessageQueue;
using TechnicalTest.Api.Mappings;
using TechnicalTest.Api.Repositories.Implementations;
using TechnicalTest.Api.Repositories.Interfaces;
using TechnicalTest.Api.Seed;
using TechnicalTest.Api.Services.Implementations;
using TechnicalTest.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();


// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConfig = builder.Configuration.GetSection("Redis");
    var config = ConfigurationOptions.Parse($"{redisConfig["Host"]}:{redisConfig["Port"]}");
    if (!string.IsNullOrEmpty(redisConfig["Password"]))
    {
        config.Password = redisConfig["Password"];
    }
    return ConnectionMultiplexer.Connect(config);
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Repositories (Scoped)
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Services (Scoped)
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICsvParser<ProductImportDto>, CsvParser<ProductImportDto>>();

// Caching
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// RabbitMQ (Singleton)
builder.Services.AddSingleton<IMessageQueueService>(sp =>
{
    var rabbitConfig = builder.Configuration.GetSection("RabbitMQ");
    var logger = sp.GetRequiredService<ILogger<RabbitMqService>>();
    
    return new RabbitMqService(
        rabbitConfig["Host"] ?? "localhost",
        int.Parse(rabbitConfig["Port"] ?? "5672"),
        rabbitConfig["Username"] ?? "guest",
        rabbitConfig["Password"] ?? "guest",
        rabbitConfig["ExchangeName"] ?? "exchange",
        logger);
});

var minioConfig = builder.Configuration.GetSection("Minio");

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = new AmazonS3Config
    {
        ServiceURL = minioConfig["Endpoint"]
            ?? throw new ArgumentNullException("Minio ServiceURL is missing"),
        ForcePathStyle = true
    };

    return new AmazonS3Client(
        minioConfig["AccessKey"]
            ?? throw new ArgumentNullException("Minio AccessKey is missing"),
        minioConfig["SecretKey"]
            ?? throw new ArgumentNullException("Minio SecretKey is missing"),
        config
    );
});

builder.Services.AddSingleton<IS3Service, S3Service>();

// Background Services
builder.Services.AddHostedService<ProductImportWorker>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TechnicalTest API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechnicalTest API v1");
    });
}

// Serilog request logging
app.UseSerilogRequestLogging();

// Global exception handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"An unexpected error occurred\"}");
    });
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    if (!dbContext.Categories.Any())
    {
        var parents = CategorySeeder.GetParentCategories();
        dbContext.Categories.AddRange(parents);
        await dbContext.SaveChangesAsync();

        var children = CategorySeeder.GetChildCategories(parents);
        dbContext.Categories.AddRange(children);
        await dbContext.SaveChangesAsync();

        Log.Information("Seeded {Count} categories", parents.Count + children.Count);
    }

    if (!dbContext.Products.Any())
    {
        var categories = dbContext.Categories.ToList();
        var products = ProductSeeder.GetProducts(categories);
        dbContext.Products.AddRange(products);
        await dbContext.SaveChangesAsync();

        Log.Information("Seeded {Count} products", products.Count);
    }
}

using (var scope = app.Services.CreateScope())
{
    var s3Service = scope.ServiceProvider.GetRequiredService<IS3Service>();
    await s3Service.CreateBucketAsync(minioConfig["BucketName"] ?? "technicaltest-bucket");
}


Log.Information("Application starting up...");
app.Run();
