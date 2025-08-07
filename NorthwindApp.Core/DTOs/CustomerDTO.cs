namespace NorthwindApp.Core.DTOs
{
    public class CustomerDTO
    {
        public required string CustomerId { get; set; }
        public required string CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CustomerCreateDto
    {
        public required string CustomerId { get; set; }
        public required string CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class CustomerFilterDto
    {
        public string? CustomerId { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortField { get; set; }
        public string? SortDirection { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CustomerUpdateDto
    {
        public required string CustomerId { get; set; }
        public required string CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool IsDeleted { get; set; }
    }
}
