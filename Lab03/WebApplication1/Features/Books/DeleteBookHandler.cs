using FluentValidation;
using WebApplication1.Persistence;

namespace WebApplication1.Features.Books;

public class DeleteBookHandler
{ private readonly IValidator<DeleteBookRequest> _validator;
    private readonly BookManagementContext  _context;
    public DeleteBookHandler(BookManagementContext context, IValidator<DeleteBookRequest> validator)
    {_validator = validator;
        _context = context;
    }

    public async Task<IResult> Handle(DeleteBookRequest request)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors=validationResult.Errors.Select(x=>x.ErrorMessage).ToList();
            return Results.BadRequest(errors);
        }
        var book = await _context.Books.FindAsync(request.Id);
        if (book == null)
        {
            return Results.NotFound();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return Results.NoContent();
    }
}