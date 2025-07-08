namespace NorthwindApp.Core.DTOs
{
    // Listeleme ve GetById için kullanılacak DTO
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
    }

    // Ekleme işlemi için kullanılacak DTO
    public class CategoryCreateDto
    {
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
    }

    // Güncelleme işlemi için kullanılacak DTO
    public class CategoryUpdateDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
