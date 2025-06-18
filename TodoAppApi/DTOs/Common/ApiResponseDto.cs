namespace TodoAppApi.DTOs.Common;

public class ApiResponseDto<T>
{
    public string Status { get; set; } = "success";
    public string Message { get; set; } = "Request completed";
    public T? Data { get; set; }

    public static ApiResponseDto<T> Success(T data, string message = "Success")
    {
        return new ApiResponseDto<T> { Status = "success", Message = message, Data = data };
    }

    public static ApiResponseDto<T> Success(string message = "Success")
    {
        return new ApiResponseDto<T> { Status = "success", Message = message, Data = default };
    }

    public static ApiResponseDto<T> Error(string message)
    {
        return new ApiResponseDto<T> { Status = "error", Message = message, Data = default };
    }
}
