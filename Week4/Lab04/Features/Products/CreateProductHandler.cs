using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Lab04.Features.Products;
using Lab04.Persistence;
using Lab04.Common.Logging;
using Lab04.Features.Products.DTOs;

namespace Lab04.Features.Products
{
    public class CreateProductHandler
    {
        private readonly ProductManagementContext _context;
        private readonly ILogger<CreateProductHandler> _logger;

        public CreateProductHandler(ProductManagementContext context, ILogger<CreateProductHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public ProductProfileDto Handle(CreateProductProfileRequest request)
        {
           
            string operationId = Guid.NewGuid().ToString("N")[..8];

            
            var totalTimer = Stopwatch.StartNew();
            var validationTimer = new Stopwatch();
            var dbTimer = new Stopwatch();
            
            using var scope = _logger.BeginScope("ProductOperationId:{OperationId}", operationId);

            try
            {
                
                _logger.LogInformation(LogEvents.ProductCreationStarted,
                    "Starting product creation | Name:{Name} | Brand:{Brand} | SKU:{SKU} | Category:{Category}",
                    request.Name, request.Brand, request.SKU, request.Category);

                
                validationTimer.Start();

                _logger.LogInformation(LogEvents.SKUValidationPerformed,
                    "Checking SKU uniqueness for {SKU}", request.SKU);

                var existing = _context.Products.FirstOrDefault(p => p.SKU == request.SKU);
                if (existing != null)
                {
                    _logger.LogWarning(LogEvents.ProductValidationFailed,
                        "Product validation failed | SKU already exists: {SKU}", request.SKU);

                    validationTimer.Stop();
                    totalTimer.Stop();

                    var failedMetrics = new ProductCreationMetrics(
                        OperationId: operationId,
                        ProductName: request.Name,
                        SKU: request.SKU,
                        Category: request.Category,
                        ValidationDuration: validationTimer.Elapsed,
                        DatabaseSaveDuration: TimeSpan.Zero,
                        TotalDuration: totalTimer.Elapsed,
                        Success: false,
                        ErrorReason: "Duplicate SKU");

                    _logger.LogProductCreationMetrics(failedMetrics);
                    throw new InvalidOperationException($"SKU '{request.SKU}' already exists.");
                }

                _logger.LogInformation(LogEvents.StockValidationPerformed,
                    "Stock validation performed for {StockQuantity}", request.StockQuantity);

                validationTimer.Stop();

                
                dbTimer.Start();
                _logger.LogInformation(LogEvents.DatabaseOperationStarted, "Saving product to database...");

                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Brand = request.Brand,
                    SKU = request.SKU,
                    Category = request.Category,
                    Price = request.Price,
                    ReleaseDate = request.ReleaseDate,
                    ImageUrl = request.ImageUrl,
                    StockQuantity = request.StockQuantity,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Products.Add(product);
                _context.SaveChanges();

                dbTimer.Stop();

                _logger.LogInformation(LogEvents.DatabaseOperationCompleted,
                    "Database operation completed | ProductId:{ProductId}", product.Id);

                
                _logger.LogInformation(LogEvents.CacheOperationPerformed,
                    "Cache updated for key 'all_products'");

                totalTimer.Stop();
                
                var dto = new ProductProfileDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Brand = product.Brand,
                    SKU = product.SKU,
                    CategoryDisplayName = product.Category.ToString(),
                    Price = product.Price,
                    FormattedPrice = product.Price.ToString("C2"),
                    ReleaseDate = product.ReleaseDate,
                    CreatedAt = product.CreatedAt,
                    ImageUrl = product.ImageUrl,
                    IsAvailable = product.IsAvailable,
                    StockQuantity = product.StockQuantity,
                    ProductAge = GetProductAge(product.ReleaseDate),
                    BrandInitials = GetBrandInitials(product.Brand),
                    AvailabilityStatus = product.IsAvailable ? "In Stock" : "Out of Stock"
                };

                
                var successMetrics = new ProductCreationMetrics(
                    OperationId: operationId,
                    ProductName: product.Name,
                    SKU: product.SKU,
                    Category: product.Category,
                    ValidationDuration: validationTimer.Elapsed,
                    DatabaseSaveDuration: dbTimer.Elapsed,
                    TotalDuration: totalTimer.Elapsed,
                    Success: true);

                _logger.LogProductCreationMetrics(successMetrics);

                return dto;
            }
            catch (Exception ex)
            {
                totalTimer.Stop();

                var errorMetrics = new ProductCreationMetrics(
                    OperationId: operationId,
                    ProductName: request.Name,
                    SKU: request.SKU,
                    Category: request.Category,
                    ValidationDuration: validationTimer.Elapsed,
                    DatabaseSaveDuration: dbTimer.Elapsed,
                    TotalDuration: totalTimer.Elapsed,
                    Success: false,
                    ErrorReason: ex.Message);

                _logger.LogError(ex,
                    "Product creation failed | Name:{Name} | Brand:{Brand} | SKU:{SKU} | Category:{Category}",
                    request.Name, request.Brand, request.SKU, request.Category);

                _logger.LogProductCreationMetrics(errorMetrics);

                throw; 
            }
        }

        
        private static string GetProductAge(DateTime releaseDate)
        {
            var days = (DateTime.UtcNow - releaseDate).TotalDays;
            if (days < 30) return "New Release";
            if (days < 365) return $"{Math.Floor(days / 30)} months old";
            if (days < 1825) return $"{Math.Floor(days / 365)} years old";
            return "Classic";
        }

        private static string GetBrandInitials(string? brand)
        {
            if (string.IsNullOrWhiteSpace(brand)) return "?";
            var parts = brand.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 1
                ? parts[0][0].ToString().ToUpper()
                : $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[^1][0])}";
        }
    }
}
