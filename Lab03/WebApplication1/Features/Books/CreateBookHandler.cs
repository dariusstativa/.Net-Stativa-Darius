using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Persistence;

namespace WebApplication1.Features.Books;

public class CreateBookHandler
{private readonly IValidator<CreateBookRequest> _validator;
    private readonly BookManagementContext _context;
    public CreateBookHandler(BookManagementContext context,IValidator<CreateBookRequest> validator)
    {
        _validator = validator;
        _context = context;
    }

    public async Task<IResult> Handle(CreateBookRequest request)
    {
        var validationResult=_validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x=>x.ErrorMessage).ToList();
            return  Results.BadRequest(errors);
        }
        var book =new Book {
            Title=request.Title, 
            Author=request.Author, 
            Year=request.Year};
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return Results.Created($"/books/{book.Id}", book);
        
    }
}