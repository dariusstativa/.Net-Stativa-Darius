using FluentValidation;
using WebApplication1.Features.Books;

namespace WebApplication1.Validators;

public class UpdateBookValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookValidator()
    {
       
        RuleFor(b => b.Id)
            .GreaterThan(0)
            .WithMessage("Book Id must be greater than 0.");

       
        RuleFor(b => b.Title)
            .MaximumLength(100)
            .When(b => !string.IsNullOrWhiteSpace(b.Title))
            .WithMessage("Title must be less than 100 characters.");

       
        RuleFor(b => b.Author)
            .MaximumLength(100)
            .When(b => !string.IsNullOrWhiteSpace(b.Author))
            .WithMessage("Author must be less than 100 characters.");

       
        RuleFor(b => b.Year)
            .InclusiveBetween(0, DateTime.Now.Year)
            .When(b => b.Year != 0)
            .WithMessage($"Year must be between 0 and {DateTime.Now.Year}.");

     
        RuleFor(b => b)
            .Must(b => !string.IsNullOrWhiteSpace(b.Title) 
                       || !string.IsNullOrWhiteSpace(b.Author) 
                       || b.Year != 0)
            .WithMessage("At least one field (Title, Author, or Year) must be provided for update.");
    }
}