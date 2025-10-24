using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Lab04.Persistence;
using Lab04.Features.Products;

namespace Lab04.Features.Products.Validators
{
    public class CreateProductProfileValidator : AbstractValidator<CreateProductProfileRequest>
    {
        private readonly ProductManagementContext _context;
        private readonly ILogger<CreateProductProfileValidator> _logger;

        
        private static readonly string[] BannedWords =
            { "banned", "inappropriate", "offensive" };

        private static readonly string[] RestrictedHomeWords =
            { "weapon", "alcohol", "violence" };

        private static readonly string[] TechKeywords =
            { "tech", "smart", "digital", "device", "electronic", "AI", "processor" };

        public CreateProductProfileValidator(
            ProductManagementContext context,
            ILogger<CreateProductProfileValidator> logger)
        {
            _context = context;
            _logger = logger;

           
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name cannot be empty.")
                .Length(1, 200).WithMessage("Product name must be between 1 and 200 characters.")
                .Must(BeValidName).WithMessage("Product name contains inappropriate content.")
                .MustAsync(BeUniqueName).WithMessage("Product name must be unique for this brand.");

            
            RuleFor(x => x.Brand)
                .NotEmpty().WithMessage("Brand cannot be empty.")
                .Length(2, 100).WithMessage("Brand must be between 2 and 100 characters.")
                .Must(BeValidBrandName).WithMessage("Brand contains invalid characters.");

            
            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage("SKU cannot be empty.")
                .Must(BeValidSKU).WithMessage("Invalid SKU format. Must be 5–20 chars, letters, digits, or hyphens.")
                .MustAsync(BeUniqueSKU).WithMessage("SKU must be unique in the system.");

            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Invalid product category.");

            
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.")
                .LessThan(10000).WithMessage("Price must be less than $10,000.");

           
            RuleFor(x => x.ReleaseDate)
                .Must(date => date <= DateTime.UtcNow).WithMessage("Release date cannot be in the future.")
                .Must(date => date.Year >= 1900).WithMessage("Release date cannot be before 1900.");

            
            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.")
                .LessThanOrEqualTo(100000).WithMessage("Stock quantity cannot exceed 100,000.");

           
            RuleFor(x => x.ImageUrl)
                .Must(BeValidImageUrl).When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
                .WithMessage("Invalid image URL or unsupported format.");

           
            RuleFor(x => x)
                .MustAsync(PassBusinessRules)
                .WithMessage("Product failed complex business validation.");
        }

       

        private bool BeValidName(string name)
        {
            return !BannedWords.Any(w =>
                name.Contains(w, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<bool> BeUniqueName(CreateProductProfileRequest req, string name, CancellationToken token)
        {
            var exists = await _context.Products
                .AnyAsync(p => p.Name.ToLower() == name.ToLower() &&
                               p.Brand.ToLower() == req.Brand.ToLower(), token);
            if (exists)
                _logger.LogWarning("Duplicate product name '{Name}' for brand '{Brand}'.", name, req.Brand);

            return !exists;
        }

        private bool BeValidBrandName(string brand)
        {
            return Regex.IsMatch(brand, @"^[A-Za-z0-9\s\-\.'`]+$");
        }

        private bool BeValidSKU(string sku)
        {
            sku = sku.Replace(" ", "");
            return Regex.IsMatch(sku, @"^[A-Za-z0-9\-]{5,20}$");
        }

        private async Task<bool> BeUniqueSKU(string sku, CancellationToken token)
        {
            var exists = await _context.Products.AnyAsync(p => p.SKU == sku, token);
            if (exists) _logger.LogWarning("Duplicate SKU found: {SKU}", sku);
            return !exists;
        }

        private bool BeValidImageUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return true;
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) return false;

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            return allowed.Any(ext => url.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }

        
        private async Task<bool> PassBusinessRules(CreateProductProfileRequest req, CancellationToken token)
        {
            
            var todayCount = await _context.Products.CountAsync(
                p => p.CreatedAt.Date == DateTime.UtcNow.Date, token);
            if (todayCount > 500)
            {
                _logger.LogWarning("Daily product addition limit exceeded ({Count}).", todayCount);
                return false;
            }

            
            if (req.Category == ProductCategory.Electronics && req.Price < 50)
            {
                _logger.LogWarning("Electronics product below minimum price: {Price}", req.Price);
                return false;
            }

          
            if (req.Category == ProductCategory.Home &&
                RestrictedHomeWords.Any(w =>
                    req.Name.Contains(w, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Home product name contains restricted term: {Name}", req.Name);
                return false;
            }

            
            if (req.Price > 500 && req.StockQuantity > 10)
            {
                _logger.LogWarning("High-value product exceeds stock limit | Price:{Price} | Stock:{Stock}",
                    req.Price, req.StockQuantity);
                return false;
            }

            return true;
        }
    }
}
