using FluentValidation;
using WebApplication1.Features.Books;

namespace WebApplication1.Validators;

public class CreateBookValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookValidator()
    {
        
        RuleFor(b => b.Title)
            .NotEmpty().WithMessage("Title must not be empty.")
            .MaximumLength(100).WithMessage("Title must be less than 100 characters.");

        
        RuleFor(b => b.Author)
            .NotEmpty().WithMessage("Author must not be empty.")
            .MaximumLength(100).WithMessage("Author must be less than 100 characters.");

        
        RuleFor(b => b.Year)
            .InclusiveBetween(0, DateTime.Now.Year + 1)
            .WithMessage($"Year must be between 0 and {DateTime.Now.Year + 1}.");
    }
}