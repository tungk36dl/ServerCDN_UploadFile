using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Infrastructure.Common
{
    /*
    200 OK	Request thành công
    400 Bad Request	Request sai cú pháp hoặc thiếu thông tin
    401 Unauthorized	Sai token hoặc chưa đăng nhập
    403 Forbidden	Đăng nhập rồi nhưng không có quyền
    404 Not Found	Không tìm thấy tài nguyên
    500 Internal Server Error	Lỗi hệ thống backend
 */
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public int StatusCode { get; set; } = 200;
        public T? Data { get; set; }
        public ApiError? Error { get; set; }

        public static ApiResponse<T> Ok(T data) => new()
        {
            Data = data,
            Success = true,
            StatusCode = 200
        };

        public static ApiResponse<T> Fail(string message, int statusCode = 400) => new()
        {
            Success = false,
            StatusCode = statusCode,
            Error = new ApiError { Message = message }
        };
    }

    public class ApiError
    {
        public string Message { get; set; } = string.Empty;
        public string? Code { get; set; }
    }
}
