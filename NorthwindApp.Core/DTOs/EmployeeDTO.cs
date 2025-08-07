namespace NorthwindApp.Core.DTOs
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }
        public required string LastName { get; set; }
        public required string FirstName { get; set; }
        public string? Title { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class EmployeeCreateDto
    {
        public required string LastName { get; set; }
        public required string FirstName { get; set; }
        public string? Title { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class EmployeeFilterDto
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Title { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortField { get; set; }
        public string? SortDirection { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class EmployeeUpdateDto
    {
        public int EmployeeId { get; set; }
        public required string LastName { get; set; }
        public required string FirstName { get; set; }
        public string? Title { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool IsDeleted { get; set; }
    }
}
