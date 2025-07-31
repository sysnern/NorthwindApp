using NorthwindApp.Core.Results;

namespace NorthwindApp.Core.Helpers
{
    /// <summary>
    /// Helper class to create consistent API responses and eliminate duplicate response creation code
    /// </summary>
    public static class ResponseHelper
    {
        #region Success Responses

        public static ApiResponse<T> Success<T>(T data, string message = "İşlem başarılı.")
        {
            return ApiResponse<T>.Ok(data, message);
        }

        public static ApiResponse<T> Created<T>(T data, string message = "Kaynak başarıyla oluşturuldu.")
        {
            return ApiResponse<T>.Created(data, message);
        }

        public static ApiResponse<string> NoContent(string message = "İşlem başarılı.")
        {
            return ApiResponse<string>.NoContent(message);
        }

        #endregion

        #region Error Responses

        public static ApiResponse<T> NotFound<T>(string message = "Kaynak bulunamadı.")
        {
            return ApiResponse<T>.NotFound(message);
        }

        public static ApiResponse<T> BadRequest<T>(string message, params string[] errors)
        {
            return ApiResponse<T>.BadRequest(errors, message);
        }

        public static ApiResponse<T> BadRequest<T>(IEnumerable<string> errors, string message = "Geçersiz istek.")
        {
            return ApiResponse<T>.BadRequest(errors, message);
        }

        public static ApiResponse<T> InternalError<T>(string message = "Sunucu hatası oluştu.")
        {
            return ApiResponse<T>.Error(message);
        }

        #endregion

        #region Entity-Specific Helpers

        public static ApiResponse<List<T>> EmptyList<T>(string entityName)
        {
            return NotFound<List<T>>($"Hiç {entityName.ToLower()} bulunamadı.");
        }

        public static ApiResponse<T> EntityNotFound<T>(string entityName)
        {
            return NotFound<T>($"{entityName} bulunamadı.");
        }

        public static ApiResponse<T> EntityCreated<T>(T data, string entityName)
        {
            return Created(data, $"{entityName} başarıyla eklendi.");
        }

        public static ApiResponse<T> EntityUpdated<T>(T data, string entityName)
        {
            return Success(data, $"{entityName} başarıyla güncellendi.");
        }

        public static ApiResponse<string> EntityDeleted(string entityName, bool isSoftDelete = false)
        {
            var message = isSoftDelete 
                ? $"{entityName} pasif hale getirildi (soft delete)."
                : $"{entityName} başarıyla silindi.";
            return NoContent(message);
        }

        public static ApiResponse<List<T>> CachedList<T>(List<T> data, string entityName)
        {
            return Success(data, $"{entityName} listesi cache'den getirildi.");
        }

        public static ApiResponse<List<T>> FreshList<T>(List<T> data, string entityName)
        {
            return Success(data, $"{entityName} listesi başarıyla getirildi.");
        }

        #endregion
    }
}