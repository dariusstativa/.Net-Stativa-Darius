using Microsoft.EntityFrameworkCore;
using FluentValidation;
using WebApplication1.Features.Books;
using WebApplication1.Middleware;
using WebApplication1.Persistence;
using WebApplication1.Validators;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<BookManagementContext>(options =>
    options.UseSqlite("Data Source=books.db"));

builder.Services.AddScoped<CreateBookHandler>();
builder.Services.AddScoped<UpdateBookHandler>();
builder.Services.AddScoped<DeleteBookHandler>();
builder.Services.AddScoped<GetByIdHandler>();
builder.Services.AddScoped<GetAllBooksHandler>();


builder.Services.AddValidatorsFromAssemblyContaining<CreateBookValidator>();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookManagementContext>();
    context.Database.EnsureCreated();
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapPost("/books", async (CreateBookRequest req, CreateBookHandler handler) =>
    await handler.Handle(req))
   .WithName("CreateBook")
   .WithOpenApi();


app.MapGet("/books", async (
    int? page,
    int? pageSize,
    string? sortBy,
    bool? asc,
    string? author,
    GetAllBooksHandler handler) =>
    await handler.Handle(new GetAllBooksRequest(
        Page: page ?? 1,
        PageSize: pageSize ?? 50,
        SortBy: sortBy,
        Asc: asc ?? true,
        Author: author)))
   .WithName("GetAllBooks")
   .WithOpenApi();


app.MapGet("/books/{id:int}", async (int id, GetByIdHandler handler) =>
    await handler.Handle(new GetByIdRequest(id)))
   .WithName("GetBookById")
   .WithOpenApi();


app.MapPut("/books/{id:int}", async (int id, UpdateBookRequest body, UpdateBookHandler handler) =>
    await handler.UpdateBook(body with { Id = id }))
   .WithName("UpdateBook")
   .WithOpenApi();


app.MapDelete("/books/{id:int}", async (int id, DeleteBookHandler handler) =>
    await handler.Handle(new DeleteBookRequest(id)))
   .WithName("DeleteBook")
   .WithOpenApi();


app.Run();
