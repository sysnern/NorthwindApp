using System.ComponentModel.DataAnnotations;

namespace NorthwindApp.Core.DTOs
{
    /// <summary>
    /// Ürün bilgilerini içeren DTO
    /// </summary>
    public class ProductDTO
    {
        /// <summary>
        /// Ürün benzersiz kimliği
        /// </summary>
        /// <example>1</example>
        public int ProductId { get; set; }

        /// <summary>
        /// Ürün adı
        /// </summary>
        /// <example>Chai</example>
        public required string ProductName { get; set; }

        /// <summary>
        /// Ürün birim fiyatı
        /// </summary>
        /// <example>18.00</example>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Ürün kategorisi ID'si
        /// </summary>
        /// <example>1</example>
        public int CategoryId { get; set; }

        /// <summary>
        /// Ürün tedarikçisi ID'si
        /// </summary>
        /// <example>1</example>
        public int SupplierId { get; set; }

        /// <summary>
        /// Ürünün silinip silinmediği (soft delete)
        /// </summary>
        /// <example>false</example>
        public bool IsDeleted { get; set; }
    }

    /// <summary>
    /// Yeni ürün oluşturma için DTO
    /// </summary>
    public class ProductCreateDto
    {
        /// <summary>
        /// Ürün adı (zorunlu)
        /// </summary>
        /// <example>Chai</example>
        [Required(ErrorMessage = "Ürün adı zorunludur")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ürün adı 2-100 karakter arasında olmalıdır")]
        public required string ProductName { get; set; }

        /// <summary>
        /// Ürün birim fiyatı (zorunlu)
        /// </summary>
        /// <example>18.00</example>
        [Required(ErrorMessage = "Birim fiyat zorunludur")]
        [Range(0, double.MaxValue, ErrorMessage = "Birim fiyat 0'dan büyük olmalıdır")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Ürün kategorisi ID'si (zorunlu)
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Kategori seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kategori seçiniz")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Ürün tedarikçisi ID'si (zorunlu)
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Tedarikçi seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir tedarikçi seçiniz")]
        public int SupplierId { get; set; }
    }

    /// <summary>
    /// Ürün filtreleme için DTO
    /// </summary>
    public class ProductFilterDto
    {
        /// <summary>
        /// Ürün adına göre filtreleme
        /// </summary>
        /// <example>Chai</example>
        public string? ProductName { get; set; }

        /// <summary>
        /// Kategori ID'sine göre filtreleme
        /// </summary>
        /// <example>1</example>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Tedarikçi ID'sine göre filtreleme
        /// </summary>
        /// <example>1</example>
        public int? SupplierId { get; set; }

        /// <summary>
        /// Minimum fiyat filtresi
        /// </summary>
        /// <example>10.00</example>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Maksimum fiyat filtresi
        /// </summary>
        /// <example>50.00</example>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// Silinmiş ürünleri dahil etme filtresi
        /// </summary>
        /// <example>false</example>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Sıralama alanı
        /// </summary>
        /// <example>ProductName</example>
        public string? SortField { get; set; }

        /// <summary>
        /// Sıralama yönü (asc/desc)
        /// </summary>
        /// <example>asc</example>
        public string? SortDirection { get; set; }

        /// <summary>
        /// Sayfa numarası (varsayılan: 1)
        /// </summary>
        /// <example>1</example>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Sayfa başına kayıt sayısı (varsayılan: 10)
        /// </summary>
        /// <example>10</example>
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Ürün güncelleme için DTO
    /// </summary>
    public class ProductUpdateDto
    {
        /// <summary>
        /// Güncellenecek ürünün ID'si
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Ürün ID'si zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir ürün ID'si giriniz")]
        public int ProductId { get; set; }

        /// <summary>
        /// Ürün adı (zorunlu)
        /// </summary>
        /// <example>Chai</example>
        [Required(ErrorMessage = "Ürün adı zorunludur")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ürün adı 2-100 karakter arasında olmalıdır")]
        public required string ProductName { get; set; }

        /// <summary>
        /// Ürün birim fiyatı (zorunlu)
        /// </summary>
        /// <example>18.00</example>
        [Required(ErrorMessage = "Birim fiyat zorunludur")]
        [Range(0, double.MaxValue, ErrorMessage = "Birim fiyat 0'dan büyük olmalıdır")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Ürün kategorisi ID'si (zorunlu)
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Kategori seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kategori seçiniz")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Ürün tedarikçisi ID'si (zorunlu)
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Tedarikçi seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir tedarikçi seçiniz")]
        public int SupplierId { get; set; }

        /// <summary>
        /// Ürünün silinip silinmediği (soft delete)
        /// </summary>
        /// <example>false</example>
        public bool IsDeleted { get; set; }
    }
}
