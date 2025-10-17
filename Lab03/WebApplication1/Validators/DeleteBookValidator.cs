using FluentValidation;
using WebApplication1.Features.Books;

namespace WebApplication1.Validators;

public class DeleteBookValidator:AbstractValidator<DeleteBookRequest>
{
    public DeleteBookValidator()
    {
        RuleFor(x=>x.Id).GreaterThan(0).WithMessage("Id must be greater than 0");
    }
}