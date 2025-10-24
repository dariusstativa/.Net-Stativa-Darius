using Lab04.Features.Products;
using Microsoft.EntityFrameworkCore;

namespace Lab04.Persistence
{
    public class ProductManagementContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ProductManagementContext(DbContextOptions<ProductManagementContext> options)
            : base(options)
        {
        }
    }
}