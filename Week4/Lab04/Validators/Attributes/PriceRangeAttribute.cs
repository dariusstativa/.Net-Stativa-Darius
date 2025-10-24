using System;
using System.ComponentModel.DataAnnotations;

namespace Lab04.Features.Products.Validators.Attributes
{
    public class PriceRangeAttribute : ValidationAttribute
    {
        private readonly decimal _min;
        private readonly decimal _max;

        public PriceRangeAttribute(double min, double max)
        {
            _min = Convert.ToDecimal(min);
            _max = Convert.ToDecimal(max);

            ErrorMessage = $"Price must be between {_min:C2} and {_max:C2}.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (decimal.TryParse(value.ToString(), out var price))
            {
                if (price >= _min && price <= _max)
                    return ValidationResult.Success;
                else
                    return new ValidationResult(ErrorMessage);
            }

            return new ValidationResult("Invalid price format.");
        }
    }
}