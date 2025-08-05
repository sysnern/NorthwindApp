namespace NorthwindApp.Core.DTOs
{
    // Listeleme ve GetById için kullanılacak DTO
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
    }

    // Ekleme işlemi için kullanılacak DTO
    public class CategoryCreateDto
    {
        public required string CategoryName { get; set; }
        public string? Description { get; set; }
    }

    // Güncelleme işlemi için kullanılacak DTO
    public class CategoryUpdateDto
    {
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
    }

    // Filtreleme için kullanılacak DTO
    public class CategoryFilterDto
    {
        public string? CategoryName { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortField { get; set; }
        public string? SortDirection { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
