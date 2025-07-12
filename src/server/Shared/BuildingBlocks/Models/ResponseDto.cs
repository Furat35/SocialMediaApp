using System.Net;

namespace BuildingBlocks.Models
{
    public class ResponseDto<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; }
        public bool IsError { get; set; }
        public T Data { get; set; }

        public static ResponseDto<T> Fail(T data, string message, HttpStatusCode statusCode)
            => new() { Data = data, ErrorMessages = [message], StatusCode = statusCode, IsError = true };

        public static ResponseDto<T> Fail(string message, HttpStatusCode statusCode)
            => new() { ErrorMessages = [message], StatusCode = statusCode, IsError = true };

        public static ResponseDto<T> Fail(List<string> messages, HttpStatusCode statusCode)
            => new() { ErrorMessages = messages, StatusCode = statusCode, IsError = true };

        public static ResponseDto<T> Success(T data, HttpStatusCode statusCode)
            => new() { StatusCode = statusCode, IsError = false, Data = data };

        public static ResponseDto<T> GenerateResponse(bool isSuccess)
            => new() { IsError = !isSuccess };
    }

    public static class ResponseExtensions
    {
        public static ResponseDto<T> Fail<T>(this ResponseDto<T> response, string message, HttpStatusCode statusCode)
            => response.IsError ? ResponseDto<T>.Fail(message, statusCode) : response;

        public static ResponseDto<T> Success<T>(this ResponseDto<T> response, T data, HttpStatusCode statusCode)
          => !response.IsError ? ResponseDto<T>.Success(data, statusCode) : response;
    }
}
