namespace NorthwindApp.Core.DTOs
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public short UnitsInStock { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class ProductCreateDto
    {
        public required string ProductName { get; set; }     //required eklendi
        public decimal UnitPrice { get; set; }  // nullable kaldırıldı
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
    }

    public class ProductFilterDto
        {
            public string? ProductName { get; set; }
            public int? CategoryId { get; set; }
            public int? SupplierId { get; set; }
            public decimal? MinPrice { get; set; }
            public decimal? MaxPrice { get; set; }
            public bool? IsDeleted { get; set; }
            public bool? Discontinued { get; set; }
            public string? SortField { get; set; }
            public string? SortDirection { get; set; }
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 10;
    }
    public class ProductUpdateDto
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }     // required eklendi
        public decimal UnitPrice { get; set; }  // nullable kaldırıldı
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public bool IsDeleted { get; set; }
    }

}
