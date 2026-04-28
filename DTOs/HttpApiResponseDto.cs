namespace Common.DTOs;

public class HttpApiResponseDto<T>
{
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static HttpApiResponseDto<T> Success(T? data, string message = "Success", string code = "SUCCESS") => new()
    {
        Message = message,
        Code = code,
        Data = data
    };

    public static HttpApiResponseDto<T> Fail(T? data, string code, string message) => new()
    {
        Message = message,
        Code = code,
        Data = data
    };
}
