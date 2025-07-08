namespace NorthwindApp.Core.DTOs
{
    public class CustomerDTO
    {
        public string CustomerID { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? Country { get; set; }
    }

    public class CustomerCreateDto
    {
        public string CustomerID { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string? ContactName { get; set; }
    }

    public class CustomerUpdateDto
    {
        public string CustomerID { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string? ContactName { get; set; }
    }
}
