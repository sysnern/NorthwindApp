namespace NorthwindApp.Core.DTOs
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public required string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public short UnitsInStock { get; set; }
        public bool Discontinued { get; set; }
        public int SupplierId { get; set; }
    }
    public class ProductCreateDto
    {
        public required string ProductName { get; set; }//required eklendi
        public decimal? UnitPrice { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
    }

    public class ProductFilterDto
        {
            public string? ProductName { get; set; }
            public int? CategoryId { get; set; }
            public decimal? MinPrice { get; set; }
            public decimal? MaxPrice { get; set; }
            public bool? Discontinued { get; set; }
        }
    public class ProductUpdateDto
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }    // required eklendi
        public decimal? UnitPrice { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
    }

}
