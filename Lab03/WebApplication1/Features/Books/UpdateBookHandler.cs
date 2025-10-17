using FluentValidation;
using WebApplication1.Persistence;

namespace WebApplication1.Features.Books;

public class UpdateBookHandler
{
    private readonly BookManagementContext _context;
    private readonly IValidator<UpdateBookRequest> _validator;

   
    public UpdateBookHandler(BookManagementContext context, IValidator<UpdateBookRequest> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<IResult> UpdateBook(UpdateBookRequest request)
    {
        
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return Results.BadRequest(errors);
        }

        
        var book = await _context.Books.FindAsync(request.Id);
        if (book == null)
            return Results.NotFound();

      
        if (!string.IsNullOrWhiteSpace(request.Author))
            book.Author = request.Author;

        if (!string.IsNullOrWhiteSpace(request.Title))
            book.Title = request.Title;

        if (request.Year != 0 && request.Year <= DateTime.Now.Year)
            book.Year = request.Year;

        await _context.SaveChangesAsync();
        return Results.Ok(book);
    }
}