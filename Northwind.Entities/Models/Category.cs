namespace NorthwindApp.Entities.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }

        // İlişkili ürünler (opsiyonel - ileride kullanılır)
        public ICollection<Product>? Products { get; set; }
    }
}
