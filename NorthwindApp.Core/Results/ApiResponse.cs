namespace NorthwindApp.Core.Results
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }       // İşlem başarılı mı?
        public string Message { get; set; }     // Bilgilendirme mesajı
        public T? Data { get; set; }            // Geri dönen veri

        public int StatusCode { get; set; }     // HTTP durum kodu (opsiyonel)

        public ApiResponse(bool success, string message, T? data = default, int statusCode = 200)
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }

        // Kısa yollar:
        public static ApiResponse<T> SuccessResponse(T? data, string message = "İşlem başarılı.", int statusCode = 200)
        {
            return new ApiResponse<T>(true, message, data, statusCode);
        }

        public static ApiResponse<T> Fail(string message = "İşlem başarısız.", int statusCode = 400)
        {
            return new ApiResponse<T>(false, message, default, statusCode);
        }
    }
}
