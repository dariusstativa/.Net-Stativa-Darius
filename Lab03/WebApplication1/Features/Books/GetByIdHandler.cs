using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using SQLitePCL;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Persistence;

namespace WebApplication1.Features.Books;

public class GetByIdHandler
{
    private readonly BookManagementContext _context;
private readonly IValidator<GetByIdRequest> _validator;
public GetByIdHandler(BookManagementContext context, IValidator<GetByIdRequest> validator)
    {
        _context = context;
        _validator = validator;
    }
    public async Task<IResult> Handle(GetByIdRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors=validationResult.Errors.Select(x=>x.ErrorMessage).ToList();
            return Results.BadRequest(errors);
        }
        var book = await _context.Books.FindAsync(request.Id);
        return book is null ? Results.NotFound() : Results.Ok(book);
    }
}