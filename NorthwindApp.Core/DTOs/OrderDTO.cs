namespace NorthwindApp.Core.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? OrderDate { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class OrderCreateDto
    {
        public string? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? OrderDate { get; set; }
    }

    public class OrderFilterDto
    {
        public int? OrderId { get; set; }
        public string? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortField { get; set; }
        public string? SortDirection { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class OrderUpdateDto
    {
        public int OrderId { get; set; }
        public string? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? OrderDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
