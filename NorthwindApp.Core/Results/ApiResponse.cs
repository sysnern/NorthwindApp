namespace NorthwindApp.Core.Results
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }       // İşlem başarılı mı?
        public string Message { get; set; }     // Bilgilendirme mesajı
        public T? Data { get; set; }            // Geri dönen veri

        public ApiResponse(bool success, string message, T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        // Kısa yollar:
        public static ApiResponse<T> SuccessResponse(T? data, string message = "İşlem başarılı.")
        {
            return new ApiResponse<T>(true, message, data);
        }

        public static ApiResponse<T> Fail(string message = "İşlem başarısız.")
        {
            return new ApiResponse<T>(false, message);
        }
    }
}
