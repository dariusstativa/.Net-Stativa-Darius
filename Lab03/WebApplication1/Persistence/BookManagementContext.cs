using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Books;

namespace WebApplication1.Persistence;

public class BookManagementContext: DbContext
{
    public DbSet<Book> Books { get; set; }
    public BookManagementContext(DbContextOptions<BookManagementContext> options) : base(options)
    {
        
    }
}