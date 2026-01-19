namespace UserService.Application.DTOs
{
    /// <summary>
    /// Standardized API response wrapper for all endpoints
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string? ErrorCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<T> CreateSuccess(string message, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> CreateBadRequest(string message, string? errorCode = null, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Data = data
            };
        }

        public static ApiResponse<T> CreateNotFound(string message, string? errorCode = null, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Data = data
            };
        }

        public static ApiResponse<T> CreateInternalServerError(string message, string? errorCode = null, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Data = data
            };
        }

        public static ApiResponse<T> CreateUnauthorized(string message, string? errorCode = null, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Data = data
            };
        }
    }

    /// <summary>
    /// API response without data (for operations that don't return data)
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse CreateSuccess(string message, object? data = null)
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse CreateBadRequest(string message, string? errorCode = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }

        public static ApiResponse CreateNotFound(string message, string? errorCode = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }

        public static ApiResponse CreateInternalServerError(string message, string? errorCode = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }

        public static ApiResponse CreateUnauthorized(string message, string? errorCode = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }
    }

    /// <summary>
    /// Error codes for better client-side handling
    /// </summary>
    public static class ErrorCodes
    {
        // Validation errors (400)
        public const string INVALID_MOBILE_NUMBER = "INVALID_MOBILE_NUMBER";
        public const string INVALID_COUNTRY_CODE = "INVALID_COUNTRY_CODE";
        public const string MISSING_REQUIRED_FIELDS = "MISSING_REQUIRED_FIELDS";
        public const string INVALID_OTP = "INVALID_OTP";
        public const string EXPIRED_OTP = "EXPIRED_OTP";
        public const string INVALID_FILE_FORMAT = "INVALID_FILE_FORMAT";
        public const string FILE_TOO_LARGE = "FILE_TOO_LARGE";
        public const string INVALID_EMAIL_FORMAT = "INVALID_EMAIL_FORMAT";
        public const string DUPLICATE_EMAIL = "DUPLICATE_EMAIL";
        public const string DUPLICATE_PHONE = "DUPLICATE_PHONE";

        // Not found errors (404)
        public const string USER_NOT_FOUND = "USER_NOT_FOUND";
        public const string PROFILE_NOT_FOUND = "PROFILE_NOT_FOUND";
        public const string RESOURCE_NOT_FOUND = "RESOURCE_NOT_FOUND";

        // Unauthorized errors (401)
        public const string INVALID_TOKEN = "INVALID_TOKEN";
        public const string TOKEN_EXPIRED = "TOKEN_EXPIRED";
        public const string ACCESS_DENIED = "ACCESS_DENIED";

        // Server errors (500)
        public const string DATABASE_ERROR = "DATABASE_ERROR";
        public const string FILE_UPLOAD_ERROR = "FILE_UPLOAD_ERROR";
        public const string EXTERNAL_SERVICE_ERROR = "EXTERNAL_SERVICE_ERROR";
        public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
    }
}
