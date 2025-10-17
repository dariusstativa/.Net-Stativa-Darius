using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Persistence;

namespace WebApplication1.Features.Books;

public class GetAllBooksHandler
{
    private readonly BookManagementContext _context;
    public GetAllBooksHandler(BookManagementContext context)
    {
        
        _context = context;
    }

    public async Task<IResult> Handle(GetAllBooksRequest request)
    {
        //throw new InvalidOperationException("Simulated database failure"); //I wanted to check if the exceptionhandling works (it does)
        int page = request.Page <= 0 ? 1 : request.Page;
        int pageSize = (request.PageSize <= 0 || request.PageSize > 100)
            ? 50
            : request.PageSize;
        IQueryable<Book> books = _context.Books;
        if (!string.IsNullOrWhiteSpace(request.Author))
        {
            books = books.Where(b => b.Author == request.Author);
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            bool asc = request.Asc;
            string sortBy = request.SortBy.ToLower();

            if (asc)
            {
                if (sortBy == "year")
                    books = books.OrderBy(b => b.Year);
                else if (sortBy == "title")
                    books = books.OrderBy(b => b.Title);
                else
                    books = books.OrderBy(b => b.Id);
            }
            else
            {
                if (sortBy == "year")
                    books = books.OrderByDescending(b => b.Year);
                else if (sortBy == "title")
                    books = books.OrderByDescending(b => b.Title);
                else
                    books = books.OrderByDescending(b => b.Id);
            }
        }
        

        var result = await books
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Results.Ok(result);

    }
}