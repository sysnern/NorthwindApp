namespace NorthwindApp.Core.DTOs
{
    public class OrderDTO
    {
        public int OrderID { get; set; }
        public required string CustomerID { get; set; }     //required to ensure non-nullability
        public int EmployeeID { get; set; }
        public DateTime? OrderDate { get; set; }
    }

    public class OrderCreateDto
    {
        public required string CustomerID { get; set; }     // required to ensure non-nullability
        public int EmployeeID { get; set; }
        public DateTime? OrderDate { get; set; }
    }

    public class OrderUpdateDto
    {
        public int OrderID { get; set; }
        public required string CustomerID { get; set; }     // required to ensure non-nullability
        public int EmployeeID { get; set; }
        public DateTime? OrderDate { get; set; }
    }
}
