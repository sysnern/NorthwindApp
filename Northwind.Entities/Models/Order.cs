using System;
using Microsoft.EntityFrameworkCore; // <— Precision attribute burada tanımlı

namespace NorthwindApp.Entities.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerID { get; set; } = null!;
        public int EmployeeID { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int? ShipVia { get; set; }

        // Freight için precision = 18, scale = 2
        [Precision(18, 2)]
        public decimal? Freight { get; set; }

        public string ShipName { get; set; } = null!;
        public string ShipAddress { get; set; } = null!;
        public string ShipCity { get; set; } = null!;
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string ShipCountry { get; set; } = null!;
    }
}