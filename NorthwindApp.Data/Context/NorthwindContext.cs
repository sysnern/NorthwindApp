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
        public DbSet<Category> Categories { get; set; }             //Database table for Categories
        public DbSet<Customer> Customers { get; set; }              //Database table for Customers
        public DbSet<Supplier> Suppliers { get; set; }              //Database table for Suppliers
        public DbSet<Employee> Employees { get; set; }              //Database table for Employees
        public DbSet<Order> Orders { get; set; }                    //Database table for Orders



    }
}
