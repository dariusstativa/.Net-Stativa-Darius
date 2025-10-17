namespace WebApplication1.Features.Books;

public record UpdateBookRequest(int Id, string? Title=null, string? Author=null, int Year=0);