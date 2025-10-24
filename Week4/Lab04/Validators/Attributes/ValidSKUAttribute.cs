using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Lab04.Features.Products.Validators.Attributes
{
    
    public class ValidSKUAttribute : ValidationAttribute, IClientModelValidator
    {
        public ValidSKUAttribute()
        {
            ErrorMessage = "Invalid SKU format. Must be 5–20 characters, alphanumeric with optional hyphens.";
        }

        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Not responsible for nulls here

            var sku = value.ToString()?.Replace(" ", "") ?? string.Empty;
            return Regex.IsMatch(sku, @"^[A-Za-z0-9\-]{5,20}$");
        }

        
        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-validsku"] = ErrorMessage;
            context.Attributes["data-val-validsku-pattern"] = "^[A-Za-z0-9\\-]{5,20}$";
        }
    }
}