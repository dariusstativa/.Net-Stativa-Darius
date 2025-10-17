namespace WebApplication1.Features.Books;

public record GetAllBooksRequest(int Page = 1, int PageSize = 10, string? SortBy = null,bool Asc=false, string? Author = null);
