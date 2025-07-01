namespace NorthwindApp.Entities.DTOs
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public required string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public short UnitsInStock { get; set; }
        public bool Discontinued { get; set; }
    }
}
