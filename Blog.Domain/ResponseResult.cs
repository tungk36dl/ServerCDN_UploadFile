using Microsoft.AspNetCore.Http;

namespace Blog.Domain
{
    /// <summary>
    /// Kết quả API chuẩn hoá.
    /// </summary>
    public class ResponseResult
    {
        #region PROPERTIES
        public bool IsSuccess { get; private set; }
        public int StatusCode { get; private set; }
        public string Message { get; private set; }
        #endregion

        #region CTORs
        protected ResponseResult(bool isSuccess, int statusCode, string? message = null)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Message = message ?? string.Empty;
        }
        #endregion

        #region SUCCESS FACTORIES
        public static ResponseResult Success(string? message = null)
            => new(true, StatusCodes.Status200OK, message);

        public static ResponseResult<T> Success<T>(T data, string? message = null)
            => new(true, StatusCodes.Status200OK, message, data);
        #endregion

        #region FAIL FACTORIES
        public static ResponseResult Fail(string message,
                                          int statusCode = StatusCodes.Status400BadRequest)
            => new(false, statusCode, message);

        public static ResponseResult<T> Fail<T>(string message,
                                                int statusCode = StatusCodes.Status400BadRequest)
            => new(false, statusCode, message, default!);

        // shortcut cho 404 / 401 nếu muốn
        public static ResponseResult NotFound(string? message = null)
            => Fail(message ?? "Not found", StatusCodes.Status404NotFound);

        public static ResponseResult Unauthorized(string? message = null)
            => Fail(message ?? "Unauthorized", StatusCodes.Status401Unauthorized);


        #endregion
    }

    /// <summary>
    /// ResponseResult có dữ liệu <typeparamref name="T"/>.
    /// </summary>
    public class ResponseResult<T> : ResponseResult
    {
        public T Data { get; set; }

        internal ResponseResult(bool isSuccess, int statusCode,
                                string? message, T data)
            : base(isSuccess, statusCode, message)
        {
            Data = data;
        }


        public static ResponseResult<T> Success(T data, string? message = null)
            => new(true, StatusCodes.Status200OK, message, data);

        public static ResponseResult<T> Fail(string message,
                                             int statusCode = StatusCodes.Status400BadRequest)
            => new(false, statusCode, message, default!);

        public static ResponseResult<T> NotFound(string? message = null)
            => Fail(message ?? "Not found", StatusCodes.Status404NotFound);

        public static ResponseResult<T> Unauthorized(string? message = null)
            => Fail(message ?? "Unauthorized", StatusCodes.Status401Unauthorized);
    }
}
