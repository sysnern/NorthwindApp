namespace NorthwindApp.Entities.Models
{
    public class Product
    {
        public int ProductID { get; set; }                      //Primary Key         
        public required string ProductName { get; set; }        //Required field

        public int SupplierID { get; set; }                     //Foreign Key to Supplier
        public int CategoryID { get; set; }                     //Foreign Key to Category

        public string? QuantityPerUnit { get; set; }            //Database can be null
        public decimal UnitPrice { get; set; }         
        public short UnitsInStock { get; set; }        
        public short UnitsOnOrder { get; set; }       
        public short ReorderLevel { get; set; }        
        public bool Discontinued { get; set; }         
    }
}
