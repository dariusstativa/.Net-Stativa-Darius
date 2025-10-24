using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Lab04.Features.Products;

namespace Lab04.Features.Products.Validators.Attributes
{
    public class ProductCategoryAttribute : ValidationAttribute
    {
        private readonly HashSet<ProductCategory> _allowedCategories;

        public ProductCategoryAttribute(params ProductCategory[] allowedCategories)
        {
            _allowedCategories = allowedCategories.ToHashSet();
            ErrorMessage = $"Category must be one of: {string.Join(", ", _allowedCategories)}.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is ProductCategory category)
            {
                if (_allowedCategories.Contains(category))
                    return ValidationResult.Success;

                return new ValidationResult(ErrorMessage);
            }

            return new ValidationResult("Invalid category type.");
        }
    }
}