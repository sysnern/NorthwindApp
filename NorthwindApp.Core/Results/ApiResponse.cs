namespace NorthwindApp.Core.Results
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }       // İşlem başarılı mı?
        public string Message { get; set; }     // Bilgilendirme mesajı
        public T? Data { get; set; }            // Geri dönen veri
        public IEnumerable<string>? Errors { get; set; } // Birden fazla hata mesajı
        public int StatusCode { get; set; }     // HTTP durum kodu

        public ApiResponse(
            bool success,
            string message,
            T? data = default,
            IEnumerable<string>? errors = null,
            int statusCode = 200)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors;
            StatusCode = statusCode;
        }

        // 200 OK
        public static ApiResponse<T> Ok(T? data, string message = "İşlem başarılı.")
            => new ApiResponse<T>(true, message, data, null, 200);

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
