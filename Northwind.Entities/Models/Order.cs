using System;
using Microsoft.EntityFrameworkCore; // <— Precision attribute burada tanımlı

namespace NorthwindApp.Entities.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; } = null!;
        public int EmployeeId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int? ShipVia { get; set; }

        // Freight için precision = 18, scale = 2
        [Precision(18, 2)]
        public decimal? Freight { get; set; }

        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation properties
        public Customer? Customer { get; set; }
        public Employee? Employee { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}