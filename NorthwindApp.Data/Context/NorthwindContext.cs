using Microsoft.EntityFrameworkCore;
using NorthwindApp.Entities.Models;

namespace NorthwindApp.Data.Context
{
    public class NorthwindContext : DbContext
    {
        public NorthwindContext(DbContextOptions<NorthwindContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = default!;    //Database table for Products
    }
}
