namespace Common.Response;

public class HttpApiResponse<T>
{
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static HttpApiResponse<T> Success(T? data, string message = "Success", string code = "SUCCESS") => new()
    {
        Message = message,
        Code = code,
        Data = data
    };

    public static HttpApiResponse<T> Fail(T? data, string code, string message) => new()
    {
        Message = message,
        Code = code,
        Data = data
    };
}
