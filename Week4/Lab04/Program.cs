using Lab04.Persistence;
using Lab04.Features.Products;
using Lab04.Features.Products.Validators;
using Lab04.Middleware;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ProductManagementContext>(options =>
    options.UseSqlite("Data Source=products.db"));


builder.Services.AddScoped<CreateProductProfileValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductProfileValidator>();

builder.Services.AddScoped<CreateProductHandler>();


builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Product API",
        Version = "v1",
        Description = "API for managing products (Lab04)",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "API Support",
            Email = "support@example.com"
        }
    });
});


builder.Services.AddControllers();

var app = builder.Build();

// Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
    options.RoutePrefix = "swagger"; // available at /swagger
});

// Correlation middleware (adds X-Correlation-Id)
app.UseMiddleware<CorrelationMiddleware>();


app.MapPost("/products", (CreateProductProfileRequest request, CreateProductHandler handler) =>
{
    var validator = new CreateProductProfileValidator(
        handler.GetType().Assembly
            .CreateInstance(typeof(ProductManagementContext).FullName!) as ProductManagementContext,
        handler.GetType().Assembly
            .CreateInstance(typeof(ILogger<CreateProductProfileValidator>).FullName!) as ILogger<CreateProductProfileValidator>
    );

    var validationResult = validator.Validate(request);
    if (!validationResult.IsValid)
    {
        var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
        return Results.BadRequest(new { Error = errors });
    }

    var result = handler.Handle(request);
    return Results.Ok(result);
})
.WithName("CreateProduct")
.WithTags("Products")
.WithDescription("Creates a new product entry with validation and logging.");


app.Run();
