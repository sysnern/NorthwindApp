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
        public DbSet<OrderDetail> OrderDetails { get; set; }        //Database table for OrderDetails

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // OrderDetail için composite primary key tanımla
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.ProductId });

            // OrderDetail UnitPrice için precision ayarı
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasPrecision(18, 2); // 18 digit, 2 decimal places

            base.OnModelCreating(modelBuilder);
        }
    }
}
