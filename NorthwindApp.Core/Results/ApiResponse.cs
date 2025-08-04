namespace NorthwindApp.Core.Results
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }       // İşlem başarılı mı?
        public string Message { get; set; }     // Bilgilendirme mesajı
        public T? Data { get; set; }            // Geri dönen veri
        public IEnumerable<string>? Errors { get; set; } // Birden fazla hata mesajı
        public int StatusCode { get; set; }     // HTTP durum kodu
        public int? TotalCount { get; set; }    // Toplam kayıt sayısı (pagination için)
        public int? Page { get; set; }          // Mevcut sayfa (pagination için)
        public int? PageSize { get; set; }      // Sayfa boyutu (pagination için)
        public int? TotalPages { get; set; }    // Toplam sayfa sayısı (pagination için)

        public ApiResponse(
            bool success,
            string message,
            T? data = default,
            IEnumerable<string>? errors = null,
            int statusCode = 200,
            int? totalCount = null,
            int? page = null,
            int? pageSize = null,
            int? totalPages = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors;
            StatusCode = statusCode;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
            TotalPages = totalPages;
        }

        // 200 OK
        public static ApiResponse<T> Ok(T? data, string message = "İşlem başarılı.")
            => new ApiResponse<T>(true, message, data, null, 200);

        // 200 OK with pagination
        public static ApiResponse<T> Ok(T? data, string message, int totalCount, int page, int pageSize)
            => new ApiResponse<T>(true, message, data, null, 200, totalCount, page, pageSize, (int)Math.Ceiling((double)totalCount / pageSize));

        // 201 Created
        public static ApiResponse<T> Created(T? data, string message = "Kaynak oluşturuldu.")
            => new ApiResponse<T>(true, message, data, null, 201);

        // 204 No Content
        public static ApiResponse<T> NoContent(string message = "İşlem başarılı; içerik yok.")
            => new ApiResponse<T>(true, message, default, null, 204);

        // 400 Bad Request
        public static ApiResponse<T> BadRequest(
            IEnumerable<string> errors,
            string message = "Geçersiz istek.")
            => new ApiResponse<T>(false, message, default, errors, 400);

        // 404 Not Found
        public static ApiResponse<T> NotFound(string message = "Kaynak bulunamadı.")
            => new ApiResponse<T>(false, message, default, null, 404);

        // 500 Internal Server Error
        public static ApiResponse<T> Error(
            string message = "Sunucu tarafında bir hata oluştu.")
            => new ApiResponse<T>(false, message, default, null, 500);
    }
}
