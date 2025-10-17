using FluentValidation;
using WebApplication1.Features.Books;

namespace WebApplication1.Validators;

public class GetByIdValidator:AbstractValidator<GetByIdRequest>
{
    public GetByIdValidator()
    {
        RuleFor(x=>x.Id).GreaterThan(0).WithMessage("Id must be greater than 0");
    }    
}