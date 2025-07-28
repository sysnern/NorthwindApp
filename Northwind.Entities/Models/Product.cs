using Microsoft.EntityFrameworkCore;

namespace NorthwindApp.Entities.Models
{
    public class Product
    {
        public int ProductID { get; set; }                      //Primary Key         
        public required string ProductName { get; set; }        //Required field

        public int SupplierId { get; set; }                     //Foreign Key to Supplier
        public int CategoryId { get; set; }                     //Foreign Key to Category

        public string? QuantityPerUnit { get; set; }            //Database can be null

        [Precision(18, 2)] 
        public decimal UnitPrice { get; set; }         
        public short UnitsInStock { get; set; }        
        public short UnitsOnOrder { get; set; }       
        public short ReorderLevel { get; set; }        
        public bool Discontinued { get; set; }         
    }
}
