namespace NorthwindApp.Entities.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
